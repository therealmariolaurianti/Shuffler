using Caliburn.Micro;
using ShufflerPro.Client;
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

    public string FunFact => FunFacts.Items.PickRandom();

    protected override void OnViewLoaded(object view)
    {
        //RunAsync(async () => await Load());
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
            throw new Exception("Failed to load library.");

        var viewModel = _shellViewModelFactory.Create(library);
        await _windowManager.ShowWindowAsync(viewModel);

        await TryCloseAsync();
    }
}

internal static class FunFacts
{
    public static List<string> Items =
    [
        "Honey never spoils. Archaeologists have found pots of honey in ancient Egyptian tombs that are over 3,000 years old and still perfectly edible.",
        "Octopuses have three hearts. Two pump blood to the gills, while the third pumps it to the rest of the body.",
        "Bananas are berries, but strawberries are not! In botanical terms, bananas fit the definition of a berry, but strawberries do not.",
        "Wombat poop is cube-shaped. This helps prevent it from rolling away and is a part of their unique digestive system.",
        "Sharks have been around longer than trees! Sharks have existed for over 400 million years, while trees appeared around 350 million years ago."
    ];
}