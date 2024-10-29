using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Factories;

public class LibraryFactory
{
    public Library Create(Guid libraryGuid)
    {
        return new Library(libraryGuid);
    }
}