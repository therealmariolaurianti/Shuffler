using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Extensions;

namespace ShufflerPro.Client.States;

public class SourceFolderState(ICollection<SourceFolder> existingSourceFolders)
{
    public ObservableCollection<SourceFolder> SourceFolders { get; } =
        existingSourceFolders.ToObservableCollection();

    public List<SourceFolder> AddedSourceFolders { get; } = [];
}