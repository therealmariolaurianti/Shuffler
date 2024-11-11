using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.States;

public class UpdateSongsState(List<Song> songs, Dictionary<string,object?> propertyDifferences)
{
    public List<Song> Songs { get; } = songs;
    public Dictionary<string, object?> PropertyDifferences { get; } = propertyDifferences;
}