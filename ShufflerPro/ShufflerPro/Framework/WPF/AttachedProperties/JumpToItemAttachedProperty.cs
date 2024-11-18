using System.Windows;
using System.Windows.Controls;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Framework.WPF.AttachedProperties;

public class JumpToItemAttachedProperty
{
    public static readonly DependencyProperty SelectingItemProperty = DependencyProperty.RegisterAttached(
        "SelectingItem",
        typeof(Song),
        typeof(JumpToItemAttachedProperty),
        new FrameworkPropertyMetadata(
            default(Song),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnSelectingItemChanged));

    private static void OnSelectingItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGrid grid || e.NewValue is not Song song)
            return;

        grid.Dispatcher.InvokeAsync(() =>
        {
            grid.UpdateLayout();
            grid.ScrollIntoView(song, null);
        });
    }

    public static Song GetSelectingItem(DependencyObject target)
    {
        return (Song)target.GetValue(SelectingItemProperty);
    }

    public static void SetSelectingItem(DependencyObject target, Song value)
    {
        target.SetValue(SelectingItemProperty, value);
    }
}