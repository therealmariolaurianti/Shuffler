using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Extensions;

namespace ShufflerPro.Client.States;

public class PlaylistState
{
    public Playlist Playlist { get; }

    public PlaylistState(IReadOnlyCollection<Song> songs, Playlist playlist)
    {
        Playlist = playlist;
        Songs = songs.ToObservableCollection();
    }

    public IReadOnlyCollection<Artist> Artists => Albums
        .Select(s => s.Artist)
        .Distinct()
        .ToReadOnlyCollection();

    public ObservableCollection<Album> Albums => Songs?
        .Where(s => s.CreatedAlbum != null)
        .Select(s => s.CreatedAlbum!)
        .Distinct()
        .ToObservableCollection() ?? [];

    public ObservableCollection<Song>? Songs { get; set; }
    public List<PlaylistIndex> Indexes => Playlist.Indexes;

    public ObservableCollection<Album> FilterAlbums(Artist? selectedArtist)
    {
        return selectedArtist is null
            ? Albums.Distinct().ToObservableCollection()
            : Albums.Where(a => a.Artist == selectedArtist).ToObservableCollection();
    }
}