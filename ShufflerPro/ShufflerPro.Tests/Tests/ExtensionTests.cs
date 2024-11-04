using System.Collections.ObjectModel;
using FluentAssertions;
using NUnit.Framework;
using ShufflerPro.Client;
using ShufflerPro.Tests.Base;

namespace ShufflerPro.Tests.Tests;

[TestFixture]
public class ExtensionTests : UnitTestBase
{
    [TestCase]
    public void Get_Files_By_Extension()
    {
        var files = _testFolderPath.GetFilesByExtension();
        files.Count.Should().Be(2);
    }
    
    [TestCase]
    public void To_Observable_Collection()
    {
        var files = _testFolderPath.GetFilesByExtension();
        var observableCollection = files.ToObservableCollection();

        observableCollection.GetType().Should().Be(typeof(ObservableCollection<string>));
    }
    
    [TestCase]
    public void To_Read_Only_Collection()
    {
        var files = _testFolderPath.GetFilesByExtension();
        var observableCollection = files.ToReadOnlyCollection();

        observableCollection.GetType().Should().Be(typeof(ReadOnlyCollection<string>));
    }

    [TestCase]
    public void Strip_Milliseconds()
    {
        var timeSpan = new TimeSpan(10, 9, 8, 7, 6, 5);
        var stripedTime = timeSpan.StripMilliseconds();

        stripedTime.ToString().Should().Be("10.09:08:07");
    }
}