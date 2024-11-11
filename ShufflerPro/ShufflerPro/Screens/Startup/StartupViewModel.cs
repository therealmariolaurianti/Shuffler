using Caliburn.Micro;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Extensions;
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
        RunAsync(async () => await Load());
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
        "Octopuses have three hearts and blue blood.",
        "Bananas are berries, but strawberries are not.",
        "Honey never spoils. Archaeologists have found pots of honey in ancient tombs that are over 3,000 years old and still edible.",
        "A day on Venus is longer than a year on Venus.",
        "Sharks existed before trees.",
        "The Eiffel Tower can grow by up to 6 inches during the summer because metal expands in the heat.",
        "Wombat poop is cube-shaped to prevent it from rolling away.",
        "A group of flamingos is called a 'flamboyance'.",
        "There's enough DNA in one human body to stretch from the Sun to Pluto and back—17 times.",
        "Cows have best friends and can become stressed when they are separated.",
        "An octopus can open jars from the inside.",
        "Sloths can hold their breath longer than dolphins can.",
        "The shortest commercial flight in the world lasts just 57 seconds, connecting two islands in Scotland.",
        "Sea otters hold hands when they sleep to keep from drifting apart.",
        "A 'jiffy' is an actual unit of time, representing 1/100th of a second.",
        "Penguins propose to their mates with a pebble.",
        "Butterflies taste with their feet.",
        "A 'butterfly' can be any color except blue—those are technically ‘blue butterflies’.",
        "Sloths only defecate once a week and they do so on the ground, risking predation.",
        "The longest hiccuping spree lasted 68 years.",
        "You can't hum while holding your nose.",
        "A group of crows is called a 'murder'.",
        "The inventor of the Pringles can is buried in one.",
        "The shortest war in history was between Britain and Zanzibar on August 27, 1896. Zanzibar surrendered after 38 minutes.",
        "The human nose can detect over 1 trillion different scents.",
        "It takes 8 minutes and 20 seconds for light from the Sun to reach Earth.",
        "Elephants are the only mammals that can't jump.",
        "Rats laugh when they’re tickled.",
        "A cloud can weigh more than 1 million pounds.",
        "Sharks can sense a drop of blood from miles away.",
        "The longest time between two twins being born is 87 days.",
        "A pineapple is a berry.",
        "The longest hiccuping spree lasted 68 years.",
        "The heart of a blue whale is the size of a small car.",
        "The unicorn is Scotland's national animal.",
        "The longest word in the English language is 189,819 letters long, and it’s the name of a protein.",
        "Cleopatra lived closer in time to the first moon landing than to the construction of the Great Pyramid of Giza.",
        "In space, astronauts cannot cry properly because there is no gravity, so tears can’t flow.",
        "A typical cloud can weigh more than 1 million pounds.",
        "There’s a species of jellyfish that is biologically immortal.",
        "The Eiffel Tower was initially intended to be a temporary structure."
    ];
}