using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.States;

public class UpdateSongsState(List<Song> songs, Dictionary<string,object?> propertyDifferences,
    AlbumArtState albumArtState)
{
    public List<Song> Songs { get; } = songs;
    public Dictionary<string, object?> PropertyDifferences { get; } = propertyDifferences;
    public AlbumArtState AlbumArtState { get; set; } = albumArtState;
}