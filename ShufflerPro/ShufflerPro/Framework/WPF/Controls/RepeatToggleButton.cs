using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ShufflerPro.Client.Enums;

namespace ShufflerPro.Framework.WPF.Controls;

public class RepeatToggleButton : ToggleButton
{
    public static readonly DependencyProperty RepeatTypeProperty = DependencyProperty.Register(
        nameof(RepeatType), typeof(RepeatType), typeof(RepeatToggleButton), new PropertyMetadata(default(RepeatType)));

    public RepeatToggleButton()
    {
        SetResourceReference(StyleProperty, typeof(ToggleButton));
        SetResourceReference(BorderBrushProperty, "MahApps.Brushes.DataGrid.Selection.Background");

        Width = 50;
        ToolTip = "Repeat";
        Margin = new Thickness(2);
        Cursor = Cursors.Hand;


        BuildContextMenu();
    }

    public RepeatType RepeatType
    {
        get => (RepeatType)GetValue(RepeatTypeProperty);
        set => SetValue(RepeatTypeProperty, value);
    }

    private void BuildContextMenu()
    {
        var contextMenu = new ContextMenu();

        var song = new MenuItem { Header = "Song", IsCheckable = true, Uid = MenuItemIds.Song };
        song.Checked += MenuItemChecked;

        var artist = new MenuItem { Header = "Artist", IsCheckable = true, Uid = MenuItemIds.Artist };
        artist.Checked += MenuItemChecked;

        var album = new MenuItem { Header = "Album", IsCheckable = true, Uid = MenuItemIds.Album };
        album.Checked += MenuItemChecked;

        contextMenu.Items.Add(song);
        contextMenu.Items.Add(artist);
        contextMenu.Items.Add(album);

        ContextMenu = contextMenu;
    }

    private void MenuItemChecked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem)
        {
            var otherMenuItems = ContextMenu!
                .Items
                .Cast<MenuItem>()
                .Where(item => item.Uid != menuItem.Uid)
                .ToList();

            if (menuItem.Uid == MenuItemIds.Song)
                RepeatType = RepeatType.Song;
            else if (menuItem.Uid == MenuItemIds.Artist)
                RepeatType = RepeatType.Artist;
            else if (menuItem.Uid == MenuItemIds.Album)
                RepeatType = RepeatType.Album;

            foreach (var otherMenuItem in otherMenuItems)
                otherMenuItem.IsChecked = false;
        }
    }

    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
        ContextMenu!.IsOpen = true;
        e.Handled = true;
    }
}

internal static class MenuItemIds
{
    public static string Song = "FB61ACD9-E760-43CC-8038-7DD2C22C41EC";
    public static string Artist = "3B4CCDFE-51DF-4916-91B7-7F16F11349BB";
    public static string Album = "AD492B1F-E941-4DCC-A3B0-D0C5771FA808";
}