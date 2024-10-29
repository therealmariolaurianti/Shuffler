using ShufflerPro.Client.Controllers;

namespace ShufflerPro.Tests;

public class UnitTestBase
{
    public FolderBrowserController CreateFolderBrowserController()
    {
        return new FolderBrowserController();
    }
}