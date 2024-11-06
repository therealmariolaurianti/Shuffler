using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

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

public class SongFilterController
{
    private readonly PlaylistController _playlistController;

    public SongFilterController(PlaylistController playlistController)
    {
        _playlistController = playlistController;
    }

    public NewResult<PlaylistState> FilterSongs(IReadOnlyCollection<Song> allSongs, Playlist? playlist)
    {
        if (playlist is null)
            return new PlaylistState(allSongs);

        var songIds = playlist.Index.Keys;
        var songs = allSongs.Where(a => songIds.Contains(a.Id)).ToList();

        return new PlaylistState(_playlistController.IndexSongs(playlist, songs));
    }

    public ObservableCollection<Song> FilterSongs(IReadOnlyCollection<Song> allSongs, string? artist, string? album)
    {
        var filteredSongs = allSongs.AsEnumerable();

        if (artist != null && album == null)
            filteredSongs = allSongs.Where(s => s.Artist == artist);
        if (artist == null && album != null)
            filteredSongs = allSongs.Where(s => s.Album == album);
        if (artist != null && album != null)
            filteredSongs = allSongs.Where(s => s.Artist == artist && s.Album == album);

        return filteredSongs
            .OrderBy(s => s.Artist)
            .ThenBy(s => s.Album)
            .ThenBy(s => s.Track)
            .ToObservableCollection();
    }

    public ObservableCollection<Song> SearchSongs(IReadOnlyCollection<Song> allSongs, string? artist, string? album,
        string? song)
    {
        var filteredSongs = allSongs.AsEnumerable();

        if (artist != null)
            filteredSongs = allSongs.Where(s => s.Artist.Contains(artist, StringComparison.OrdinalIgnoreCase));
        if (album != null)
            filteredSongs = allSongs.Where(s => s.Album.Contains(album, StringComparison.OrdinalIgnoreCase));
        if (song != null)
            filteredSongs = allSongs.Where(s =>
                s.Title != null && s.Title.Contains(song, StringComparison.OrdinalIgnoreCase));

        return filteredSongs.ToObservableCollection();
    }
}