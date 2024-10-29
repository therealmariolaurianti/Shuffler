using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Factories;

namespace ShufflerPro.Tests;

public class UnitTestBase
{
    public FolderBrowserController CreateFolderBrowserController()
    {
        return new FolderBrowserController();
    }


    public MediaController CreateMediaController()
    {
        return new MediaController(new ArtistFactory(), new AlbumFactory());
    }
}