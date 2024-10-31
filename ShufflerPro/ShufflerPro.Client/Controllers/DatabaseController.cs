using ShufflerPro.Client.Entities;
using ShufflerPro.Database;
using ShufflerPro.Database.Interfaces;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class DatabaseController
{
    private readonly IDatabasePath _databasePath;
    private readonly LocalDatabase _localDatabase;

    public DatabaseController(LocalDatabase localDatabase, IDatabasePath databasePath)
    {
        _localDatabase = localDatabase;
        _databasePath = databasePath;
    }

    public async Task<NewResult<List<string>>> LoadSources()
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var sources = connection.GetCollection<Source>();
            var sourcePaths = await sources.FindAll().ConfigureAwait(true);

            return sourcePaths.Select(s => s.FolderPath).ToList();       
        }
    }

    public async Task<NewResult<NewUnit>> InsertSource(string folderPath)
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var source = new Source(folderPath);
            var sourceCollection = connection.GetCollection<Source>();
            await sourceCollection.Insert(source);
        }

        return NewUnit.Default;
    }
}