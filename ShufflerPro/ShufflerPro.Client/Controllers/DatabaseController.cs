using System.Linq.Expressions;
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

    public async Task<NewResult<List<Source>>> LoadSources()
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var sources = connection.GetCollection<Source>();
            var sourcePaths = await sources.FindAll().ConfigureAwait(true);

            return sourcePaths.ToList();
        }
    }

    public async Task<NewResult<NewUnit>> InsertSource(SourceFolderState state)
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            foreach (var addedSourceFolder in state.AddedSourceFolders)
            {
                var sourceCollection = connection.GetCollection<Source>();

                var source = new Source(addedSourceFolder.FullPath);
                await sourceCollection.Insert(source);
            }
        }

        return NewUnit.Default;
    }

    public async Task<NewResult<NewUnit>> DeleteSource(SourceFolder sourceFolder)
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var sourceCollection = connection.GetCollection<Source>();
            foreach (var folder in sourceFolder.Flatten())
            {
                if(folder.IsRoot)
                    continue;
                var result = await sourceCollection.Delete(_deleteExpression(folder));
                if (result == 0)
                    return NewResultExtensions.CreateFail<NewUnit>(new Exception("Failed to delete source"));
            }
        }

        return NewUnit.Default;
    }

    private Expression<Func<Source, bool>> _deleteExpression(SourceFolder sourceFolder)
    {
        return s => s.FolderPath == sourceFolder.FullPath;
    }
}