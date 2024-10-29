using System.Collections.ObjectModel;

namespace ShufflerPro.Client.Entities;

public class Library(Guid libraryGuid)
{
    public Guid Id { get; set; } = libraryGuid;

    public ObservableCollection<Artist> Artists { get; } = new();
    public IReadOnlyCollection<Song> Songs => Artists.SelectMany(al => al.Albums.SelectMany(s => s.Songs)).ToList();
    public IReadOnlyCollection<Album> Albums => Artists.SelectMany(al => al.Albums).ToList();

    public string Summary
    {
        get
        {
            var totalSongs = Songs.Count;
            var totalTime = TimeSpan.FromTicks(Songs.Sum(s => s.Duration?.Ticks) ?? 0);

            return $"{totalSongs} songs, {totalTime:mm':'ss} total time";
        }
    }

    public void AddArtists(IReadOnlyCollection<Artist> artists)
    {
        foreach (var artist in artists)
        {
            var existingArtist = Artists.SingleOrDefault(a => a.Name == artist.Name);
            if (existingArtist is not null)
                existingArtist.Albums.AddRange(artist.Albums);
            else
                Artists.Add(artist);
        }
    }
}