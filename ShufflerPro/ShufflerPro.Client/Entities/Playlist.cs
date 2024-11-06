using System.Collections.ObjectModel;

namespace ShufflerPro.Client.Entities;

public class Playlist(string name)
{
    public string Name { get; set; } = name;
    public List<Song> Songs { get; set; } = new();
    public SongIndex SongIndex { get; set; } = new();

    public IReadOnlyCollection<Artist> Artists => Albums
        .Select(s => s.Artist)
        .Distinct()
        .ToReadOnlyCollection();

    public ObservableCollection<Album> Albums => Songs
        .Where(s => s.CreatedAlbum != null)
        .Select(s => s.CreatedAlbum!)
        .Distinct()
        .ToObservableCollection();
}

public class SongIndex
{
    public Dictionary<Guid, int> Index { get; set; } = new();
}