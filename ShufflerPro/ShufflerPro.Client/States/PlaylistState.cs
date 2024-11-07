using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Extensions;

namespace ShufflerPro.Client.States;

public class PlaylistState(IReadOnlyCollection<Song> songs)
{
    public IReadOnlyCollection<Artist> Artists => Albums
        .Select(s => s.Artist)
        .Distinct()
        .ToReadOnlyCollection();

    public ObservableCollection<Album> Albums => Songs
        .Where(s => s.CreatedAlbum != null)
        .Select(s => s.CreatedAlbum!)
        .Distinct()
        .ToObservableCollection();

    public ObservableCollection<Song> Songs { get; set; } = songs.ToObservableCollection();

    public ObservableCollection<Album> FilterAlbums(Artist? selectedArtist)
    {
        return selectedArtist is null
            ? Albums
            : Albums.Where(a => a.Artist == selectedArtist).ToObservableCollection();
    }
}