﻿using FluentAssertions;
using NUnit.Framework;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Tests;

[TestFixture]
public class SourceTreeTests : UnitTestBase
{
    [TestCase]
    public void Load_Single_Folder_One_Level()
    {
        var folderBrowserController = CreateSourceFolderController();

        folderBrowserController
            .BuildSourceFolders(@"C:\Level1\", new List<SourceFolder>())
            .Do(sourceFolders =>
            {
                var root = sourceFolders.First();

                root.Header.Should().Be("C:");
                root.IsRoot.Should().Be(true);

                root.Items[0].Header.Should().Be("Level1");
            });
    }

    [TestCase]
    public void Load_Single_Folder_Multiple_Levels()
    {
        var folderBrowserController = CreateSourceFolderController();

        folderBrowserController
            .BuildSourceFolders(@"C:\Level1\Level2\Level3\Level4", new List<SourceFolder>())
            .Do(sourceFolders =>
            {
                var root = sourceFolders.First();

                root.Header.Should().Be("C:");
                root.IsRoot.Should().Be(true);

                root.Items[0].Header.Should().Be("Level1");
                root.Items[0].FullPath.Should().Be(@"C:\Level1");

                root.Items[0].Items[0].Header.Should().Be("Level2");
                root.Items[0].Items[0].FullPath.Should().Be(@"C:\Level1\Level2");

                root.Items[0].Items[0].Items[0].Header.Should().Be("Level3");
                root.Items[0].Items[0].Items[0].FullPath.Should().Be(@"C:\Level1\Level2\Level3");

                root.Items[0].Items[0].Items[0].Items[0].Header.Should().Be("Level4");
                root.Items[0].Items[0].Items[0].Items[0].FullPath.Should().Be(@"C:\Level1\Level2\Level3\Level4");
            });
    }

    [TestCase]
    public void Load_Multiple_Folders_One_Level()
    {
        var folderBrowserController = CreateSourceFolderController();
        var existingSourceFolders = new List<SourceFolder>();

        folderBrowserController
            .BuildSourceFolders(@"C:\Level1\", existingSourceFolders)
            .Do(sourceFolders =>
            {
                var root = sourceFolders.First();

                root.Header.Should().Be("C:");
                root.IsRoot.Should().Be(true);

                root.Items[0].Header.Should().Be("Level1");
            });

        folderBrowserController
            .BuildSourceFolders(@"C:\Level1_Second\", existingSourceFolders)
            .Do(sourceFolders =>
            {
                var root = sourceFolders.First();

                root.Header.Should().Be("C:");
                root.IsRoot.Should().Be(true);

                root.Items[1].Header.Should().Be("Level1_Second");
            });
    }

    [TestCase]
    public void Load_Multiple_Folders_Same_Parent()
    {
        var folderBrowserController = CreateSourceFolderController();
        var existingSourceFolders = new List<SourceFolder>();

        folderBrowserController
            .BuildSourceFolders(@"C:\Level1\SubLevel1", existingSourceFolders)
            .Do(sourceFolders =>
            {
                var root = sourceFolders.First();

                root.Header.Should().Be("C:");
                root.IsRoot.Should().Be(true);

                root.Items[0].Header.Should().Be("Level1");
                root.Items[0].Items[0].Header.Should().Be("SubLevel1");
            });

        folderBrowserController
            .BuildSourceFolders(@"C:\Level1\SubLevel2", existingSourceFolders)
            .Do(sourceFolders =>
            {
                var root = sourceFolders.First();

                root.Header.Should().Be("C:");
                root.IsRoot.Should().Be(true);

                root.Items.Count.Should().Be(1);

                root.Items[0].Header.Should().Be("Level1");
                root.Items[0].Items[1].Header.Should().Be("SubLevel2");
            });
    }

    [TestCase]
    public void Load_Top_Level_Folder_With_Children()
    {
        var folderBrowserController = CreateSourceFolderController();
        var existingSourceFolders = new List<SourceFolder>();

        folderBrowserController
            .BuildSourceFolders(@"C:\UnitTest", existingSourceFolders)
            .Do(sourceFolders =>
            {
                var root = sourceFolders.First();

                root.Header.Should().Be("C:");
                root.IsRoot.Should().Be(true);

                root.Items[0].Header.Should().Be("UnitTest");

                root.Items[0].Items[0].Header.Should().Be("Folder_1");
                root.Items[0].Items[0].Items[0].Header.Should().Be("SubFolder_1");
                root.Items[0].Items[0].Items[1].Header.Should().Be("SubFolder_2");

                root.Items[0].Items[1].Header.Should().Be("Folder_2");
                root.Items[0].Items[1].Items[0].Header.Should().Be("SubFolder_1");
                root.Items[0].Items[1].Items[1].Header.Should().Be("SubFolder_2");

                root.Items[0].Items[2].Header.Should().Be("Folder_3");
                root.Items[0].Items[2].Items[0].Header.Should().Be("SubFolder_1");
                root.Items[0].Items[2].Items[1].Header.Should().Be("SubFolder_2");
            });
    }

    [TestCase]
    public void Build_Paths_From_Source_Folders()
    {
        var mediaController = CreateMediaController();

        var sourceFolders = new List<SourceFolder>();

        var root = new SourceFolder(@"C:", "C:", true, null);
        sourceFolders.Add(root);

        var level1 = new SourceFolder(@"UnitTest", @"C:\UnitTest", false, root);
        root.Items.Add(level1);

        var level2 = new SourceFolder(@"Folder_1", @"C:\UnitTest\Folder_1", false, level1);
        level1.Items.Add(level2);

        var library = new Library();

        mediaController
            .LoadFromFolderPath(sourceFolders, library)
            .Do(_ =>
            {
                root.IsProcessed.Should().Be(true);
                level1.IsProcessed.Should().Be(true);
                level2.IsProcessed.Should().Be(true);
            });
    }

    [TestCase]
    public void Remove_Source_Folder_Remove_Root()
    {
        var mediaController = CreateMediaController();

        var sourceFolders = new List<SourceFolder>();

        var root = new SourceFolder(@"C:", "C:", true, null);
        sourceFolders.Add(root);

        var level1 = new SourceFolder(@"UnitTest", @"C:\UnitTest", false, root);
        root.Items.Add(level1);

        var level2 = new SourceFolder(@"Folder_1", @"C:\UnitTest\Folder_1", false, level1);
        level1.Items.Add(level2);

        var library = new Library();

        var song = new Song(null, "C:\\UnitTest\\Folder_1");
        var album = new Album("Artist_1", "Album_1", new List<Song> { song });
        var artist = new Artist("Artist_1", new List<Album> { album });

        song.CreatedAlbum = album;
        album.CreatedArtist = artist;

        library.AddArtists(new[] { artist });

        mediaController
            .LoadFromFolderPath(sourceFolders, library)
            .Do(_ =>
            {
                var sourceFolderController = CreateSourceFolderController();
                var removeFolder = sourceFolders.First();

                sourceFolderController.Remove(library, removeFolder);

                library.SourceFolders.Count.Should().Be(0);
                library.Artists.Count.Should().Be(0);
                library.Albums.Count.Should().Be(0);
                library.Songs.Count.Should().Be(0);
            });
    }

    [TestCase]
    public void Remove_Source_Folder_Remove_Child()
    {
        var mediaController = CreateMediaController();

        var sourceFolders = new List<SourceFolder>();

        var root = new SourceFolder(@"C:", "C:", true, null);
        sourceFolders.Add(root);

        var level1 = new SourceFolder(@"UnitTest", @"C:\UnitTest", false, root);
        root.Items.Add(level1);

        var level2 = new SourceFolder(@"Folder_1", @"C:\UnitTest\Folder_1", false, level1);
        level1.Items.Add(level2);

        var library = new Library();

        mediaController
            .LoadFromFolderPath(sourceFolders, library)
            .Do(_ =>
            {
                var sourceFolderController = CreateSourceFolderController();
                var removeFolder = sourceFolders.First();

                sourceFolderController.Remove(library, removeFolder);

                Assert.Fail();
            });
    }
}