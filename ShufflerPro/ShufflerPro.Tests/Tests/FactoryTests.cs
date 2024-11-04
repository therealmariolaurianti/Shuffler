﻿using System.Collections.ObjectModel;
using FluentAssertions;
using NUnit.Framework;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Factories;
using ShufflerPro.Tests.Base;

namespace ShufflerPro.Tests.Tests;

[TestFixture]
public class FactoryTests : UnitTestBase
{
    [TestCase]
    public void Create_Artist()
    {
        var artistFactory = new ArtistFactory();
        var artist = artistFactory.Create("Artist_1", []);

        artist.Should().NotBeNull();
        artist.Albums.Count.Should().Be(0);
        artist.Name.Should().Be("Artist_1");
    }

    [TestCase]
    public void Create_Album()
    {
        var artistFactory = new ArtistFactory();
        var artist = artistFactory.Create("Artist_1", []);

        var albumFactory = new AlbumFactory();
        var album = albumFactory.Create(artist, "Album_1", []);

        artist.Albums.Add(album);

        album.Should().NotBeNull();
        album.Artist.Should().Be(artist);
        album.Songs.Count.Should().Be(0);
        album.Name.Should().Be("Album_1");

        artist.Albums.Count.Should().Be(1);
    }

    [TestCase]
    public void Create_Song()
    {
        var song = SongFactory.Create($@"{_testFolderPath}\Actual\CXIN.mp3");
        song.Artist.Should().NotBeNull();
    }

    [TestCase]
    public void Create_Song_Queue()
    {
        var previousSong = SongFactory.Create("Path_previous");
        var currentSong = SongFactory.Create("Path_current");
        var nextSong = SongFactory.Create("Path_next");

        var collection = new ObservableCollection<Song> { previousSong, currentSong, nextSong };

        var songQueueFactory = new SongQueueFactory();
        var songQueue = songQueueFactory.Create(currentSong, collection);

        songQueue.PreviousSong.Should().Be(previousSong);
        songQueue.CurrentSong.Should().Be(currentSong);
        songQueue.NextSong.Should().Be(nextSong);
    }

    [TestCase]
    public void Create_Song_Queue_No_Previous()
    {
        var currentSong = SongFactory.Create("Path_current");
        var nextSong = SongFactory.Create("Path_next");

        var collection = new ObservableCollection<Song> { currentSong, nextSong };

        var songQueueFactory = new SongQueueFactory();
        var songQueue = songQueueFactory.Create(currentSong, collection);

        songQueue.PreviousSong.Should().Be(null);
        songQueue.CurrentSong.Should().Be(currentSong);
        songQueue.NextSong.Should().Be(nextSong);
    }

    [TestCase]
    public void Create_Song_Queue_No_Next()
    {
        var previousSong = SongFactory.Create("Path_previous");
        var currentSong = SongFactory.Create("Path_current");

        var collection = new ObservableCollection<Song> { previousSong, currentSong };

        var songQueueFactory = new SongQueueFactory();
        var songQueue = songQueueFactory.Create(currentSong, collection);

        songQueue.PreviousSong.Should().Be(previousSong);
        songQueue.CurrentSong.Should().Be(currentSong);
        songQueue.NextSong.Should().Be(null);
    }
}