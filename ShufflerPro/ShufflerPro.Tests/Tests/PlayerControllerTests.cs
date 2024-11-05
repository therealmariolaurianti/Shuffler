using System.Collections.ObjectModel;
using NAudio.Wave;
using NUnit.Framework;
using ShufflerPro.Client;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Factories;
using ShufflerPro.Tests.Base;

namespace ShufflerPro.Tests.Tests;

[TestFixture]
public class PlayerControllerTests : UnitTestBase
{
    [TestCase]
    public void Test_Skip_With_Random_Song_Queue()
    {
        var playerController = new PlayerController(new WaveOutEvent())
        {
            PlayerDisposed = null,
            SongChanged = null
        };
        
        var song1 = SongFactory.Create("random_1");
        var song2 = SongFactory.Create("random_2");
        var song3 = SongFactory.Create("random_3");
        var song4 = SongFactory.Create("random_4");
        var song5 = SongFactory.Create("random_5");

        var allSongs = new ObservableCollection<Song> { song1, song2, song3, song4, song5 };
        var songStack = new SongStack();
        
        var randomSongQueueFactory = new RandomSongQueueFactory();
        var randomSongQueue = randomSongQueueFactory.Create(null, allSongs, songStack);
        
        playerController.Skip(randomSongQueue);
    }
}