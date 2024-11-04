using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Factories;

namespace ShufflerPro.Tests.Base;

public class UnitTestBase
{
    public SourceFolderController CreateSourceFolderController()
    {
        return new SourceFolderController(null);
    }
    
    //TODO get relative path
    internal const string _testFolderPath = "D:\\Projects\\Shuffler\\Tests";

    public MediaController CreateMediaController()
    {
        return new MediaController(new ArtistFactory(), new AlbumFactory());
    }
}