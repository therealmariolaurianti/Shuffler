using Caliburn.Micro;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Framework.WPF;
using ShufflerPro.Result;
using ShufflerPro.Screens.Shell;

namespace ShufflerPro.Screens.Startup;

public class StartupViewModel : ViewModelBase
{
    private readonly LibraryController _libraryController;
    private readonly IShellViewModelFactory _shellViewModelFactory;
    private readonly IWindowManager _windowManager;

    public StartupViewModel(
        LibraryController libraryController,
        IShellViewModelFactory shellViewModelFactory,
        IWindowManager windowManager)
    {
        _libraryController = libraryController;
        _shellViewModelFactory = shellViewModelFactory;
        _windowManager = windowManager;
    }

    protected override void OnViewLoaded(object view)
    {
        RunAsync(async () => await Load());
    }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        DisplayName = "Shuffler";
        return base.OnInitializeAsync(cancellationToken);
    }

    private async Task Load()
    {
        Library? library = null;
        await Task.Run(async () =>
        {
            await _libraryController.Initialize()
                .IfFail(_ => library = null)
                .IfSuccess(createdLibrary => library = createdLibrary);
        }).ConfigureAwait(true);

        if (library is null)
        {
            
        }
        else
        {
            var viewModel = _shellViewModelFactory.Create(library);
            await _windowManager.ShowWindowAsync(viewModel);

            await TryCloseAsync();
        }
    }
}