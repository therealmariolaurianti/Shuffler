using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client;

public class SourceFolderState(ICollection<SourceFolder> existingSourceFolders)
{
    public ObservableCollection<SourceFolder> SourceFolders { get; } =
        existingSourceFolders.ToObservableCollection();

    public List<SourceFolder> AddedSourceFolders { get; } = new();
}