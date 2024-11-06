using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Database;
using ShufflerPro.Result;

namespace ShufflerPro.Client;

public class SourceFolderState(ICollection<SourceFolder> existingSourceFolders)
{
    public ObservableCollection<SourceFolder> SourceFolders { get; } =
        existingSourceFolders.ToObservableCollection();

    public List<SourceFolder> AddedSourceFolders { get; } = new();

    public NewResult<SourceFolderState> WireIds(List<Source> sources)
    {
        try
        {
            foreach (var sourceFolder in AddedSourceFolders)
            {
                var source = sources.Single(s => s.FolderPath == sourceFolder.FullPath);
                sourceFolder.SetId(new LocalDatabaseKey(source.Id));
            }

            return this;
        }
        catch (Exception e)
        {
            return NewResultExtensions.CreateFail<SourceFolderState>(e);
        }
    }
}