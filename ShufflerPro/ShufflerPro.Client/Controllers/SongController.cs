using ShufflerPro.Client.Entities;
using ShufflerPro.Result;
using TagLib;
using TagLib.Id3v2;
using File = TagLib.File;

namespace ShufflerPro.Client.Controllers;

public class AlbumArtState
{
    public AlbumArtState(byte[]? albumArt, bool albumArtChanged)
    {
        AlbumArt = albumArt;
        AlbumArtChanged = albumArtChanged;
    }

    public byte[]? AlbumArt { get; }
    public bool AlbumArtChanged { get; }
}

public class SongController
{
    public async Task<NewResult<NewUnit>> Update(ItemTracker<Song> itemTracker,
        AlbumArtState albumArtState)
    {
        if (!itemTracker.PropertyDifferences.Any() && !albumArtState.AlbumArtChanged)
            return NewUnit.Default;

        var song = File.Create(itemTracker.Item.Path);
        foreach (var propertyDifference in itemTracker.PropertyDifferences)
        {
            var result = UpdateProperty(song, propertyDifference);
            if (result.Fail)
                return result;
        }

        if (albumArtState.AlbumArtChanged)
            UpdateAlbumArt(song, albumArtState.AlbumArt);

        song.Save();

        return await NewUnit.DefaultAsync;
    }

    private void UpdateAlbumArt(File song, byte[]? albumArt)
    {
        var cover = new AttachmentFrame
        {
            Type = PictureType.FrontCover,
            Data = albumArt,
            TextEncoding = StringType.UTF16
        };

        song.Tag.Pictures = [cover];
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