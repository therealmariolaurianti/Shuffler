using FluentAssertions;
using LiteDB;
using NUnit.Framework;
using ShufflerPro.Client.Entities;
using ShufflerPro.Database;
using ShufflerPro.Result;

namespace ShufflerPro.Tests;

[TestFixture]
public class DatabaseTests : UnitTestBase
{
    private const string _testFolderPath = "D:\\Projects\\Shuffler\\Tests";
    
    [TestCase]
    public void Test_Find_Root()
    {
        var databasePath = new DatabasePath(_testFolderPath);
        databasePath.Start();
        
        databasePath.Path.Should().Be($@"{_testFolderPath}\local.db");
    }

    [TestCase]
    public void Test_Find_Root_No_Root()
    {
        var databasePath = new DatabasePath(Path.GetTempPath());
        var result = databasePath.Start();
        result.Success.Should().Be(false);
    }

    [TestCase]
    public void Test_Create_Local_Database()
    {
        var databasePath = new DatabasePath(_testFolderPath);
        databasePath.Start();
        
        if(File.Exists(databasePath.Path))
            File.Delete(databasePath.Path);

        var localDatabase = new LocalDatabase();
        
        using (var connection = localDatabase.CreateConnection(databasePath.Path))
        {
            connection.Should().NotBeNull();
            Assert.That(File.Exists(databasePath.Path));
        }
    }
    
    [TestCase]
    public void Test_Get_Collection_From_Database()
    {
        var databasePath = new DatabasePath(_testFolderPath);
        databasePath.Start();
        
        if(File.Exists(databasePath.Path))
            File.Delete(databasePath.Path);

        var localDatabase = new LocalDatabase();
        
        using (var connection = localDatabase.CreateConnection(databasePath.Path))
        {
            var sourceCollection = connection.GetCollection<Source>();
            sourceCollection.Should().NotBeNull();
        }
    }
    
    [TestCase]
    public async Task Test_Insert_Into_Collection()
    {
        var databasePath = new DatabasePath(_testFolderPath);
        databasePath.Start();
        
        if(File.Exists(databasePath.Path))
            File.Delete(databasePath.Path);

        var localDatabase = new LocalDatabase();
        
        using (var connection = localDatabase.CreateConnection(databasePath.Path))
        {
            var sourceCollection = connection.GetCollection<Source>();

            var source = new Source(Path.GetTempPath());
            var insertResult = await sourceCollection.Insert(source);
            insertResult.Value.Should().NotBeNull();
        }
    }
    
    [TestCase]
    public async Task Test_Delete_From_Collection()
    {
        var databasePath = new DatabasePath(_testFolderPath);
        databasePath.Start();
        
        if(File.Exists(databasePath.Path))
            File.Delete(databasePath.Path);

        var localDatabase = new LocalDatabase();
        
        using (var connection = localDatabase.CreateConnection(databasePath.Path))
        {
            var sourceCollection = connection.GetCollection<Source>();

            var folderPath = Path.GetTempPath();
            var source = new Source(folderPath);
            
            await sourceCollection.Insert(source);
            
            var deleteResult = await sourceCollection.Delete(s => s.FolderPath == folderPath);
            deleteResult.Should().Be(1);
        }
    }
    
    [TestCase]
    public async Task Test_Find_All_In_Collection()
    {
        var databasePath = new DatabasePath(_testFolderPath);
        databasePath.Start();
        
        if(File.Exists(databasePath.Path))
            File.Delete(databasePath.Path);

        var localDatabase = new LocalDatabase();
        
        using (var connection = localDatabase.CreateConnection(databasePath.Path))
        {
            var sourceCollection = connection.GetCollection<Source>();

            await sourceCollection.DeleteAll();

            var source = new Source(Path.GetTempPath());
            var source2 = new Source(Path.GetTempPath());
            
            await sourceCollection.Insert(source);
            await sourceCollection.Insert(source2);

            var findAll = (await sourceCollection.FindAll()).ToList();
            findAll.Count.Should().Be(2);
        }
    }
}