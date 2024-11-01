using System.Collections.ObjectModel;

namespace ShufflerPro.Client.Entities;

public class Library
{
    public ObservableCollection<SourceFolder> SourceFolders { get; set; } = new();
    public ObservableCollection<Artist> Artists { get; } = new();
    public IReadOnlyCollection<Song> Songs => Albums.SelectMany(al => al.Songs).ToList();
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
}