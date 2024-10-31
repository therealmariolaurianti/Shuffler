using System.Linq.Expressions;
using LiteDB;
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

    public async Task<NewResult<NewUnit>> DeleteSource(SourceFolder sourceFolder)
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var sourceCollection = connection.GetCollection<Source>();
            Expression<Func<Source,bool>> expression = s => s.FolderPath == sourceFolder.FullPath;
            
            var result = await sourceCollection.Delete(expression);
            if (result == 0)
                return NewResultExtensions.CreateFail<NewUnit>(new Exception("Failed to delete source"));
        }

        return NewUnit.Default;
    }
}