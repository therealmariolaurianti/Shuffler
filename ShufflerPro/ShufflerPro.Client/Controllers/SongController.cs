﻿using System.Reflection;
using System.Xml.XPath;
using ShufflerPro.Client.States;
using ShufflerPro.Result;
using TagLib;
using TagLib.Id3v2;
using File = TagLib.File;

namespace ShufflerPro.Client.Controllers;

public class SongController
{
    public async Task<NewResult<NewUnit>> Update(UpdateSongsState state)
    {
        foreach (var stateSong in state.Songs)
        {
            var result = await Update(stateSong.Path!, state.PropertyDifferences, state.AlbumArtState);
            if (result.Fail)
                return result;
        }

        return await NewUnit.DefaultAsync;
    }
    
    private async Task<NewResult<NewUnit>> Update(string songPath, Dictionary<string, object?> propertyDifferences,
        AlbumArtState albumArtState)
    {
        if (propertyDifferences.Count == 0 && !albumArtState.AlbumArtChanged)
            return NewUnit.Default;

        var song = File.Create(songPath);
        foreach (var propertyDifference in propertyDifferences)
        {
            var result = UpdateProperty(song, propertyDifference);
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

    private NewResult<NewUnit> UpdateProperty(File song, KeyValuePair<string, object?> propertyDifference)
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
                    song.Tag.Title = propertyDifference.Value as string;
                }
                    break;
                case "Artist":
                {
                    song.Tag.Performers = null;
                    song.Tag.AlbumArtists = null;

                    song.Tag.AlbumArtists = [(string)propertyDifference.Value!];
                    song.Tag.Performers = [(string)propertyDifference.Value!];
                }
                    break;
                case "Album":
                {
                    song.Tag.Album = null;
                    song.Tag.Album = propertyDifference.Value as string;
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
}