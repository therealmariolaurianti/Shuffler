using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.States;

public class UpdateSongsState(List<Song> songs, Dictionary<string,object?> propertyDifferences,
    AlbumArtState albumArtState, Library library)
{
    public List<Song> Songs { get; } = songs;
    public Dictionary<string, object?> PropertyDifferences { get; } = propertyDifferences;
    public AlbumArtState AlbumArtState { get; set; } = albumArtState;
    public Library Library { get; set; } = library;
}