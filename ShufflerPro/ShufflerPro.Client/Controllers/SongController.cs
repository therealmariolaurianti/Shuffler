using ShufflerPro.Client.Entities;
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

    public SongController(ArtistFactory artistFactory)
    {
        _artistFactory = artistFactory;
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
        if(stateSong.CreatedAlbum?.Name != albumValue)
            UpdateExistingSongCollections(stateSong, library);

        var existingArtist = library.Artists.SingleOrDefault(a => a.Name == artistValue);
        if (existingArtist != null)
        {
            var existingAlbum = existingArtist.Albums.SingleOrDefault(a => a.Name == albumValue);
            if (existingAlbum != null)
            {
                if(existingAlbum.Songs.All(s => s.Id != stateSong.Id))
                    existingAlbum.Songs.Add(stateSong);
            }
            else
            {
                var album = new Album(existingArtist, stateSong.Album, [stateSong]);
                existingArtist.Albums.Add(album);
            }
        }
        else
        {
            var newArtist = _artistFactory.Create(artistValue, []);
            var album = new Album(newArtist, stateSong.Album, [stateSong]);

            newArtist.Albums.Add(album);
            stateSong.CreatedAlbum = album;
            library.Artists.Add(newArtist);
        }
    }

    private static void UpdateExistingSongCollections(Song stateSong, Library library)
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
}