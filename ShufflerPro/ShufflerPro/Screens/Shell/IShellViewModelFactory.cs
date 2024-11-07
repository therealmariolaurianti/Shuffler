using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Screens.Shell;

public interface IShellViewModelFactory : IFactory
{
    ShellViewModel Create(Library library);
}