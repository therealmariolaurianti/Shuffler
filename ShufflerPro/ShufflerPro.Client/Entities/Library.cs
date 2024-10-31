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
            var totalSpan = Songs
                .Aggregate(TimeSpan.Zero, (sumSoFar, nextMyObject) =>
                {
                    if (nextMyObject.Duration.HasValue)
                        return sumSoFar + nextMyObject.Duration.Value;
                    return sumSoFar;
                });
            return $"{totalSongs} songs, {totalSpan.Duration().StripMilliseconds()} total time";
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