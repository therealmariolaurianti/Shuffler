﻿using NAudio.Wave;
using ShufflerPro.Client.Entities;
using Timer = System.Timers.Timer;

namespace ShufflerPro.Client.Controllers;

public class PlayerController(WaveOutEvent outEvent, CancellationTokenSource cancellationToken)
    : IDisposable
{
    private AudioFileReader? _audioFileReader;
    private CancellationTokenSource? _cancellationToken = cancellationToken;
    private WaveOutEvent? _outEvent = outEvent;

    public bool Playing => _outEvent?.PlaybackState == PlaybackState.Playing;
    public bool IsCompleted { get; set; }

    public Action<Song> SongChanged;

    public void Dispose()
    {
        _outEvent?.Dispose();
        _audioFileReader?.Dispose();
        _cancellationToken?.Dispose();

        _outEvent = null;
        _audioFileReader = null;
        _cancellationToken = null;
    }

    private void OnSongComplete(Song currentSong, Artist selectedArtist, Album? album)
    {
        Song? nextTrack;

        if (album is not null)
        {
            var tracks = album.Songs.OrderBy(s => s.Track);
            var index = album.Songs.IndexOf(currentSong) + 1;
            nextTrack = tracks.Skip(index).FirstOrDefault();
        }
        else
        {
            var songs = selectedArtist.Songs.OrderBy(s => s.Album).ThenBy(s => s.Track).ToList();
            var index = songs.IndexOf(currentSong) + 1;
            nextTrack = songs.Skip(index).FirstOrDefault();
        }

        if (nextTrack is null)
            Dispose();
        else
            SongChanged.Invoke(nextTrack);
    }

    public void ReInitialize()
    {
        Dispose();

        _outEvent = new WaveOutEvent();
        _cancellationToken = new CancellationTokenSource();
        IsCompleted = false;
    }

    public void Cancel()
    {
        _cancellationToken?.Cancel();
        ReInitialize();
    }

    public bool PlaySong(Artist selectedArtist, Album? selectedAlbum, Song song)
    {
        try
        {
            using (_audioFileReader = new AudioFileReader(song.Path))
            {
                _outEvent ??= new WaveOutEvent();

                _outEvent.Init(_audioFileReader);
                _outEvent.Play();

                DelayAction(_audioFileReader.TotalTime.TotalMilliseconds, () => OnSongComplete(song,
                    selectedArtist, selectedAlbum));
            }
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public static void DelayAction(double millisecond, Action action)
    {
        var timer = new Timer();

        timer.Elapsed += delegate
        {
            action.Invoke();
            timer.Stop();
        };

        timer.Interval = millisecond;
        timer.Start();
    }
}