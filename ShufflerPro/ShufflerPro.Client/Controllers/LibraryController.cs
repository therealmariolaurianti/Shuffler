using ShufflerPro.Client.Entities;
using ShufflerPro.Database;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class LibraryController
{
    private readonly LocalDatabase _localDatabase;

    public LibraryController(LocalDatabase localDatabase)
    {
        _localDatabase = localDatabase;
    }

    public async Task<NewResult<Library?>> LoadLibrary(Guid libraryGuid)
    {
        using (var connection = _localDatabase.CreateConnection(""))
        {
        }

        return null;
    }
}