using System.Collections.ObjectModel;

namespace ShufflerPro.Client.Entities;

public class Library
{
    public Guid Id { get; set; }
    
    public ReadOnlyCollection<Artist> Artists { get; } = new(new List<Artist>());
    public IReadOnlyCollection<Song> Songs => Artists.SelectMany(al => al.Albums.SelectMany(s => s.Songs)).ToList();
    public IReadOnlyCollection<Album> Albums => Artists.SelectMany(al => al.Albums).ToList();
}