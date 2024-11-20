﻿using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Framework.WPF;
using ShufflerPro.Result;
using ShufflerPro.Web;
using ShufflerPro.Web.Radio;

namespace ShufflerPro.Controllers;

public class SourceTreeController
{
    private readonly RadioController _radioController;

    public SourceTreeController(RadioController radioController)
    {
        _radioController = radioController;
    }

    private static SourceTreeViewItem Library => new()
    {
        SourceFolder = new SourceFolder("Library", string.Empty,
            true, null),
        Id = LibraryId,
        IsTopLevelItem = true,
        Header = "Library"
    };

    private SourceTreeViewItem RadioStations => new()
    {
        SourceFolder = new SourceFolder("RadioStations", string.Empty,
            true, null),
        Id = RadioStationId,
        IsTopLevelItem = true,
        Header = "Radio Stations"
    };

    private static Guid LibraryId => Guid.Parse("1A6F50DB-A562-479E-B833-19B0F8635F31");
    private static Guid RadioStationId => Guid.Parse("7B584231-D29F-4621-8385-0E535005D106");

    public NewResult<ObservableCollection<SourceTreeViewItem>> Initialize()
    {
        var items = new ObservableCollection<SourceTreeViewItem> { Library, RadioStations };
        return BuildUpRadioStations(items);
    }

    private NewResult<ObservableCollection<SourceTreeViewItem>> BuildUpRadioStations(
        ObservableCollection<SourceTreeViewItem> items)
    {
        return _radioController
            .GetStations()
            .Bind(radioStations =>
            {
                return GetRadioStations(items)
                    .Do(radioSourceTreeItem =>
                    {
                        foreach (var radioStation in radioStations)
                            radioSourceTreeItem.Items.Add(BuildRadioSourceTreeItem(radioStation));
                    })
                    .Map(_ => items);
            });
    }

    private static SourceTreeViewItem BuildRadioSourceTreeItem(IRadioStation radioStation)
    {
        var sourceFolder = new SourceFolder(radioStation.Name, radioStation.Url, false, null);
        var radioSourceTreeItem = new SourceTreeViewItem
        {
            SourceFolder = sourceFolder,
            Id = Guid.NewGuid(),
            Header = radioStation.Name
        };

        return radioSourceTreeItem;
    }

    public NewResult<SourceTreeViewItem> GetLibrary(ObservableCollection<SourceTreeViewItem> sourceTreeItems)
    {
        return sourceTreeItems.Single(sti => sti.Id == LibraryId);
    }

    public NewResult<SourceTreeViewItem> GetRadioStations(ObservableCollection<SourceTreeViewItem> sourceTreeItems)
    {
        return sourceTreeItems.Single(sti => sti.Id == RadioStationId);
    }
}