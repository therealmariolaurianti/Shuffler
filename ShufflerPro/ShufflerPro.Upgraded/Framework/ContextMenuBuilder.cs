using System.Windows;
using System.Windows.Controls;

namespace ShufflerPro.Upgraded.Framework;

public class ContextMenuBuilder
{
    private Action? _browseAction;
    private Action? _removeAction;

    public ContextMenu BuildContextMenu(Action? browseAction, Action? removeAction)
    {
        _browseAction = browseAction;
        _removeAction = removeAction;

        var contextMenu = new ContextMenu();
        var menuItem = new MenuItem
        {
            Header = "Browse"
        };

        menuItem.Click += OnBrowseClick;
        contextMenu.Items.Add(menuItem);

        contextMenu.Items.Add(new Separator());

        var menuItem2 = new MenuItem
        {
            Header = "Remove"
        };
        menuItem2.Click += OnRemoveClick;

        contextMenu.Items.Add(menuItem2);

        return contextMenu;
    }

    private void OnRemoveClick(object sender, RoutedEventArgs e)
    {
        _removeAction?.Invoke();
    }

    private void OnBrowseClick(object sender, RoutedEventArgs e)
    {
        _browseAction?.Invoke();
    }
}