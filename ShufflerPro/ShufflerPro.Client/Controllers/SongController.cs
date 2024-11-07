using ShufflerPro.Client.Entities;
using ShufflerPro.Result;
using File = TagLib.File;

namespace ShufflerPro.Client.Controllers;

public class SongController
{
    public async Task<NewResult<NewUnit>> Update(ItemTracker<Song> itemTracker)
    {
        if (!itemTracker.PropertyDifferences.Any())
            return NewUnit.Default;

        var song = File.Create(itemTracker.Item.Path);
        foreach (var propertyDifference in itemTracker.PropertyDifferences)
            UpdateProperty(song, propertyDifference);

        song.Save();

        return await NewUnit.DefaultAsync;
    }

    private void UpdateProperty(File song, KeyValuePair<string, object?> propertyDifference)
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
                song.Tag.Track = (uint)(propertyDifference.Value ?? 0);
            }
                break;
        }
    }
}