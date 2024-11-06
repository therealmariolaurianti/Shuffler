using System.Collections.ObjectModel;

namespace ShufflerPro.Client.Entities;

public class Library
{
    public ObservableCollection<Playlist> Playlists { get; set; } = new();
    public ObservableCollection<SourceFolder> SourceFolders { get; set; } = new();
    public ObservableCollection<Artist> Artists { get; } = new();
    public IReadOnlyCollection<Song> Songs => Albums.SelectMany(al => al.Songs).ToList();
    public IReadOnlyCollection<Album> Albums => Artists.SelectMany(al => al.Albums).ToList();
}