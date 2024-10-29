using FluentAssertions;
using NUnit.Framework;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Tests;

[TestFixture]
public class SourceTreeTests : UnitTestBase
{
    [TestCase]
    public void Load_Single_Folder_One_Level()
    {
        var folderBrowserController = CreateFolderBrowserController();

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
        var folderBrowserController = CreateFolderBrowserController();

        folderBrowserController
            .BuildSourceFolders(@"C:\Level1\Level2\Level3\Level4", new List<SourceFolder>())
            .Do(sourceFolders =>
            {
                var root = sourceFolders.First();
                
                root.Header.Should().Be("C:");
                root.IsRoot.Should().Be(true);

                root.Items[0].Header.Should().Be("Level1");
                root.Items[0].Items[0].Header.Should().Be("Level2");
                root.Items[0].Items[0].Items[0].Header.Should().Be("Level3");
                root.Items[0].Items[0].Items[0].Items[0].Header.Should().Be("Level4");
            });
    }
    
    [TestCase]
    public void Load_Multiple_Folders_One_Level()
    {
        var folderBrowserController = CreateFolderBrowserController();
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
        var folderBrowserController = CreateFolderBrowserController();
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
        var folderBrowserController = CreateFolderBrowserController();
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
}