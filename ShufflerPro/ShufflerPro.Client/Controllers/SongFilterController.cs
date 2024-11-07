﻿using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Extensions;
using ShufflerPro.Client.States;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

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

        var songIds = playlist.Indexes.Select(i => i.SongId);
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

    public ObservableCollection<Song> FilterPlaylist(PlaylistState playlistState, string? artist, string? album)
    {
        var playlistSongs = playlistState.Songs;
        if (artist is null && album is null)
            return playlistSongs;
        
        var filteredSongs = playlistSongs.AsEnumerable();
        if (artist != null && album == null)
            filteredSongs = playlistSongs.Where(s => s.Artist == artist);
        if (artist == null && album != null)
            filteredSongs = playlistSongs.Where(s => s.Album == album);
        if (artist != null && album != null)
            filteredSongs = playlistSongs.Where(s => s.Artist == artist && s.Album == album);

        return filteredSongs.ToObservableCollection();
    }
}