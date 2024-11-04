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
    public void Create_Song_Queue()
    {
        
    }
}