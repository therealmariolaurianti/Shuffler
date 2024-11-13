﻿using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Factories;
using ShufflerPro.Client.States;
using ShufflerPro.Result;
using TagLib;
using TagLib.Id3v2;
using File = TagLib.File;

namespace ShufflerPro.Client.Controllers;

public class SongController
{
    private readonly ArtistFactory _artistFactory;
    private readonly DatabaseController _databaseController;

    public SongController(ArtistFactory artistFactory, DatabaseController databaseController)
    {
        _artistFactory = artistFactory;
        _databaseController = databaseController;
    }

    public async Task<NewResult<NewUnit>> Update(UpdateSongsState state)
    {
        foreach (var stateSong in state.Songs)
        {
            var result = await Update(stateSong, state);
            if (result.Fail)
                return result;
        }

        return await NewUnit.DefaultAsync;
    }

    private async Task<NewResult<NewUnit>> Update(Song stateSong, UpdateSongsState state)
    {
        var propertyDifferences = state.PropertyDifferences;
        var albumArtState = state.AlbumArtState;

        if (propertyDifferences.Count == 0 && !albumArtState.AlbumArtChanged)
            return NewUnit.Default;

        var song = File.Create(stateSong.Path);
        foreach (var propertyDifference in propertyDifferences)
        {
            var result = UpdateProperty(song, stateSong, state.Library, propertyDifference);
            if (result.Fail)
                return result;
        }

        if (albumArtState.AlbumArtChanged)
        {
            var result = UpdateAlbumArt(song, albumArtState.AlbumArt);
            if (result.Fail)
                return result;
        }

        song.Save();

        return await NewUnit.DefaultAsync;
    }

    private NewResult<NewUnit> UpdateAlbumArt(File song, byte[]? albumArt)
    {
        return NewResultExtensions.Try(() =>
        {
            try
            {
                var cover = new AttachmentFrame
                {
                    Type = PictureType.FrontCover,
                    Data = albumArt,
                    TextEncoding = StringType.UTF16
                };

                song.Tag.Pictures = [cover];

                return NewUnit.Default;
            }
            catch (Exception e)
            {
                return NewResultExtensions.CreateFail<NewUnit>(e);
            }
        });
    }

    private NewResult<NewUnit> UpdateProperty(File song, Song stateSong, Library library,
        KeyValuePair<string, object?> propertyDifference)
    {
        return NewResultExtensions.Try(() =>
        {
            switch (propertyDifference.Key)
            {
                case "Genre":
                {
                    song.Tag.Genres = null;
                    song.Tag.Genres = [(string)propertyDifference.Value!];
                }
                    break;
                case "Title":
                {
                    song.Tag.Title = null;
                    song.Tag.Title = (string)propertyDifference.Value!;
                }
                    break;
                case "Artist":
                {
                    song.Tag.Performers = null;
                    song.Tag.AlbumArtists = null;

                    var value = (string)propertyDifference.Value!;

                    song.Tag.AlbumArtists = [value];
                    song.Tag.Performers = [value];

                    HandleUpdateArtist(stateSong, library, value, stateSong.Album);
                }
                    break;
                case "Album":
                {
                    var value = (string)propertyDifference.Value!;

                    song.Tag.Album = null;
                    song.Tag.Album = value;

                    HandleUpdateArtist(stateSong, library, stateSong.Artist, value);
                }
                    break;
                case "Track":
                {
                    song.Tag.Track = Convert.ToUInt32(propertyDifference.Value);
                }
                    break;
            }

            return NewUnit.Default;
        });
    }

    private void HandleUpdateArtist(Song stateSong, Library library, string artistValue, string albumValue)
    {
        if (stateSong.CreatedAlbum?.Name != albumValue)
            UpdateSongCollections(stateSong, library);

        var existingArtist = library.Artists.SingleOrDefault(a => a.Name == artistValue);
        if (existingArtist != null)
        {
            var existingAlbum = existingArtist.Albums.SingleOrDefault(a => a.Name == albumValue);
            if (existingAlbum != null)
            {
                if (existingAlbum.Songs.All(s => s.Id != stateSong.Id))
                {
                    existingAlbum.Songs.Add(stateSong);
                    stateSong.CreatedAlbum = existingAlbum;
                }
            }
            else
            {
                var album = new Album(existingArtist, albumValue, [stateSong]);
                existingArtist.Albums.Add(album);
                stateSong.CreatedAlbum = album;
            }
        }
        else
        {
            var newArtist = _artistFactory.Create(artistValue, []);
            var album = new Album(newArtist, albumValue, [stateSong]);

            newArtist.Albums.Add(album);
            library.Artists.Add(newArtist);

            stateSong.CreatedAlbum = album;
        }
    }

    private void UpdateSongCollections(Song stateSong, Library library)
    {
        stateSong.CreatedAlbum!.Songs.Remove(stateSong);
        if (stateSong.CreatedAlbum.Songs.Count == 0)
        {
            var createdAlbumArtist = stateSong.CreatedAlbum.Artist;
            createdAlbumArtist.Albums.Remove(stateSong.CreatedAlbum);
            if (createdAlbumArtist.Albums.Count == 0)
                library.Artists.Remove(createdAlbumArtist);
        }
    }

    public async Task<NewResult<NewUnit>> Remove(List<Song> songs, Library library)
    {
        foreach (var song in songs)
        {
            var result = await Remove(song, library);
            if (result.Fail)
                return result;
        }
        
        return await NewUnit.DefaultAsync; 
    }

    private async Task<NewResult<NewUnit>> Remove(Song selectedSong, Library library)
    {
        return await _databaseController.RemoveSong(selectedSong.Id)
            .Bind(_ =>
            {
                var songArtist = library.Artists.SingleOrDefault(a => a.Name == selectedSong.CreatedAlbum?.Artist.Name);
                if (songArtist is null)
                    return NewResultExtensions.CreateFail<NewUnit>("Song could not be removed");
                
                var album = songArtist.Albums.SingleOrDefault(a => a.Name == selectedSong.CreatedAlbum?.Name);
                if(album is null)
                    return NewResultExtensions.CreateFail<NewUnit>("Song could not be removed");
                
                UpdateSongCollections(selectedSong, library);
                
                return NewUnit.Default;
            });
    }
}