﻿using FluentAssertions;
using LiteDB;
using NUnit.Framework;
using ShufflerPro.Client.Entities;
using ShufflerPro.Database;
using ShufflerPro.Tests.Base;

namespace ShufflerPro.Tests.Tests;

[TestFixture]
public class DatabaseTests : UnitTestBase
{
    [TestCase]
    public void Find_Root()
    {
        var databasePath = new DatabasePath(_testFolderPath);
        databasePath.Start();

        databasePath.Path.Should().Be($@"{_testFolderPath}\local.db");
    }

    [TestCase]
    public void Find_Root_No_Root()
    {
        var databasePath = new DatabasePath(Path.GetTempPath());
        var result = databasePath.Start();
        result.Success.Should().Be(false);
    }

    [TestCase]
    public void Create_Local_Database()
    {
        var databasePath = new DatabasePath(_testFolderPath);
        databasePath.Start();

        if (File.Exists(databasePath.Path))
            File.Delete(databasePath.Path);

        var localDatabase = new LocalDatabase();

        using (var connection = localDatabase.CreateConnection(databasePath.Path))
        {
            connection.Should().NotBeNull();
            Assert.That(File.Exists(databasePath.Path));
        }
    }

    [TestCase]
    public void Get_Collection_From_Database()
    {
        var databasePath = new DatabasePath(_testFolderPath);
        databasePath.Start();

        if (File.Exists(databasePath.Path))
            File.Delete(databasePath.Path);

        var localDatabase = new LocalDatabase();

        using (var connection = localDatabase.CreateConnection(databasePath.Path))
        {
            var sourceCollection = connection.GetCollection<Source>();
            sourceCollection.Should().NotBeNull();
        }
    }

    [TestCase]
    public async Task Insert_Into_Collection()
    {
        var databasePath = new DatabasePath(_testFolderPath);
        databasePath.Start();

        if (File.Exists(databasePath.Path))
            File.Delete(databasePath.Path);

        var localDatabase = new LocalDatabase();

        using (var connection = localDatabase.CreateConnection(databasePath.Path))
        {
            var sourceCollection = connection.GetCollection<Source>();

            var source = new Source(Path.GetTempPath(), ObjectId.NewObjectId());
            var insertResult = await sourceCollection.Insert(source);
            insertResult.Value.Should().NotBeNull();
        }
    }

    [TestCase]
    public async Task Delete_From_Collection()
    {
        var databasePath = new DatabasePath(_testFolderPath);
        databasePath.Start();

        if (File.Exists(databasePath.Path))
            File.Delete(databasePath.Path);

        var localDatabase = new LocalDatabase();

        using (var connection = localDatabase.CreateConnection(databasePath.Path))
        {
            var sourceCollection = connection.GetCollection<Source>();
            await sourceCollection.DeleteAll();

            var folderPath = Path.GetTempPath();
            var source = new Source(folderPath, ObjectId.NewObjectId());

            var id = await sourceCollection.Insert(source);

            var deleteResult = await sourceCollection.Delete(id);

            var items = (await sourceCollection.FindAll()).ToList();

            items.Count.Should().Be(0);
            deleteResult.Should().Be(true);
        }
    }

    [TestCase]
    public async Task Find_All_In_Collection()
    {
        var databasePath = new DatabasePath(_testFolderPath);
        databasePath.Start();

        if (File.Exists(databasePath.Path))
            File.Delete(databasePath.Path);

        var localDatabase = new LocalDatabase();

        using (var connection = localDatabase.CreateConnection(databasePath.Path))
        {
            var sourceCollection = connection.GetCollection<Source>();

            await sourceCollection.DeleteAll();

            var source = new Source(Path.GetTempPath(), ObjectId.NewObjectId());
            var source2 = new Source(Path.GetTempPath(), ObjectId.NewObjectId());

            await sourceCollection.Insert(source);
            await sourceCollection.Insert(source2);

            var findAll = (await sourceCollection.FindAll()).ToList();
            findAll.Count.Should().Be(2);
        }
    }
}