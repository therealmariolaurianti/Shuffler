using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace ShufflerPro.Framework.WPF.Watermark;

internal class WatermarkAdorner : Adorner
{
    private readonly ContentPresenter contentPresenter;

    public WatermarkAdorner(UIElement adornedElement, object watermark) :
        base(adornedElement)
    {
        IsHitTestVisible = false;

        contentPresenter = new ContentPresenter
        {
            Content = watermark,
            Opacity = 0.75
        };

        if (Control is ItemsControl and not ComboBox)
        {
            contentPresenter.VerticalAlignment = VerticalAlignment.Center;
            contentPresenter.HorizontalAlignment = HorizontalAlignment.Center;
        }

        var binding = new Binding("IsVisible")
        {
            Source = adornedElement,
            Converter = new BooleanToVisibilityConverter()
        };

        SetBinding(VisibilityProperty, binding);
    }

    protected override int VisualChildrenCount => 1;

    private Control Control => (Control)AdornedElement;

    protected override Visual GetVisualChild(int index)
    {
        return contentPresenter;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        contentPresenter.Measure(Control.RenderSize);
        return Control.RenderSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        contentPresenter.Arrange(new Rect(finalSize));
        return finalSize;
    }
}