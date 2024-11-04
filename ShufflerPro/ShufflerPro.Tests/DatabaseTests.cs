using FluentAssertions;
using NUnit.Framework;
using ShufflerPro.Database;

namespace ShufflerPro.Tests;

[TestFixture]
public class DatabaseTests : UnitTestBase
{
    private const string _testFolderPath = "D:\\Projects\\Shuffler\\Tests";
    
    [TestCase]
    public void Test_Find_Root()
    {
        var databasePath = new DatabasePath(_testFolderPath);
        var result = databasePath.Start();
        if (result.Fail)
            Assert.Fail();
        
        var rootPath = databasePath.Path;
        if (string.IsNullOrEmpty(rootPath))
            Assert.Fail();
        
        databasePath.Path.Should().Be($@"{_testFolderPath}\local.db");
    }

    [TestCase]
    public void Test_Find_Root_No_Root()
    {
        var databasePath = new DatabasePath(Path.GetTempPath());
        var result = databasePath.Start();
        if (result.Fail)
            Assert.Pass();
        
        Assert.Fail();
    }
}