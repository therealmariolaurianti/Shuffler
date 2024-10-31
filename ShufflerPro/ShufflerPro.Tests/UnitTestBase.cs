using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Factories;
using ShufflerPro.Database;

namespace ShufflerPro.Tests;

public class UnitTestBase
{
    public SourceFolderController CreateFolderBrowserController()
    {
        return new SourceFolderController();
    }


    public MediaController CreateMediaController()
    {
        return new MediaController(new ArtistFactory(), new AlbumFactory());
    }
}