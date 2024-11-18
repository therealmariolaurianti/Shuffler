using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Action = Caliburn.Micro.Action;

namespace ShufflerPro.Framework.WPF.Controls;

public class ShufflerDataGrid : DataGrid
{
    public static readonly DependencyProperty SelectedItemsListProperty =
        DependencyProperty.Register(nameof(SelectedItemsList), typeof(IList), typeof(ShufflerDataGrid),
            new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public ShufflerDataGrid()
    {
        SelectionChanged += CustomDataGrid_SelectionChanged;
    }

    public IList SelectedItemsList
    {
        get => (IList)GetValue(SelectedItemsListProperty);
        set => SetValue(SelectedItemsListProperty, value);
    }

    private void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedItemsList = SelectedItems;
    }
}