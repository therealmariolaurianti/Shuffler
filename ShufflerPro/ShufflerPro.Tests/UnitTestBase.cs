using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Factories;
using ShufflerPro.Database;

namespace ShufflerPro.Tests;

public class UnitTestBase
{
    public SourceFolderController CreateSourceFolderController()
    {
        return new SourceFolderController(null);
    }


    public MediaController CreateMediaController()
    {
        return new MediaController(new ArtistFactory(), new AlbumFactory());
    }
}