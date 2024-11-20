using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace ShufflerPro.Framework.WPF.Watermark;

public static class WatermarkService
{
    public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
        "Watermark",
        typeof(object),
        typeof(WatermarkService),
        new FrameworkPropertyMetadata(null, OnWatermarkChanged));

    private static readonly Dictionary<object, ItemsControl> itemsControls = new();

    public static object GetWatermark(DependencyObject d)
    {
        return d.GetValue(WatermarkProperty);
    }

    public static void SetWatermark(DependencyObject d, object value)
    {
        d.SetValue(WatermarkProperty, value);
    }

    private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Control)d;
        control.Loaded += Control_Loaded;

        if (d is ComboBox or TextBox)
        {
            control.GotKeyboardFocus += Control_GotKeyboardFocus;
            control.LostKeyboardFocus += Control_Loaded;
        }

        if (d is ItemsControl itemsControl and not ComboBox)
        {
            itemsControl.ItemContainerGenerator.ItemsChanged += ItemsChanged;
            itemsControls.Add(itemsControl.ItemContainerGenerator, itemsControl);

            var prop = DependencyPropertyDescriptor
                .FromProperty(ItemsControl.ItemsSourceProperty, itemsControl.GetType());
            prop.AddValueChanged(itemsControl, ItemsSourceChanged!);
        }
    }

    private static void Control_GotKeyboardFocus(object sender, RoutedEventArgs e)
    {
        var c = (Control)sender;
        if (ShouldShowWatermark(c)) RemoveWatermark(c);
    }

    private static void Control_Loaded(object sender, RoutedEventArgs e)
    {
        var control = (Control)sender;
        if (ShouldShowWatermark(control)) ShowWatermark(control);
    }

    private static void ItemsSourceChanged(object sender, EventArgs e)
    {
        var c = (ItemsControl)sender;
        if (c.ItemsSource != null)
        {
            if (ShouldShowWatermark(c))
                ShowWatermark(c);
            else
                RemoveWatermark(c);
        }
        else
        {
            ShowWatermark(c);
        }
    }

    private static void ItemsChanged(object sender, ItemsChangedEventArgs e)
    {
        if (itemsControls.TryGetValue(sender, out var control))
        {
            if (ShouldShowWatermark(control))
                ShowWatermark(control);
            else
                RemoveWatermark(control);
        }
    }

    private static void RemoveWatermark(UIElement control)
    {
        var layer = AdornerLayer.GetAdornerLayer(control);
        if (layer != null)
        {
            Adorner[]? adorners = layer.GetAdorners(control);
            if (adorners == null)
                return;

            foreach (var adorner in adorners)
                if (adorner is WatermarkAdorner)
                {
                    adorner.Visibility = Visibility.Hidden;
                    layer.Remove(adorner);
                }
        }
    }

    private static void ShowWatermark(Control control)
    {
        var layer = AdornerLayer.GetAdornerLayer(control);
        layer?.Add(new WatermarkAdorner(control, GetWatermark(control)));
    }

    private static bool ShouldShowWatermark(Control c)
    {
        return c switch
        {
            ComboBox box => box.Text == string.Empty,
            TextBoxBase => (c as TextBox)?.Text == string.Empty,
            ItemsControl control => control.Items.Count == 0,
            _ => false
        };
    }
}