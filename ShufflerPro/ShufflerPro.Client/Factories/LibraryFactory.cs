using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Factories;

public class LibraryFactory
{
    public Library Create(ObservableCollection<SourceFolder> sourceFolders)
    {
        return new Library
        {
            SourceFolders = sourceFolders
        };
    }
}