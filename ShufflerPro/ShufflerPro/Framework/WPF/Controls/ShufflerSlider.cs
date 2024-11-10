using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ShufflerPro.Framework.WPF.Controls;

public class ShufflerSlider : Slider
{
    public static readonly DependencyProperty SelectedValueProperty =
        DependencyProperty.Register(
            nameof(SelectedValue),
            typeof(double),
            typeof(ShufflerSlider),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty StartingValueProperty = DependencyProperty.Register(
        nameof(StartingValue), typeof(double), typeof(ShufflerSlider),
        new PropertyMetadata(default(double)));

    private double _startingValue;

    public double StartingValue
    {
        get => (double)GetValue(StartingValueProperty);
        set => SetValue(StartingValueProperty, value);
    }

    public double SelectedValue
    {
        get => (double)GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    protected override void OnThumbDragStarted(DragStartedEventArgs e)
    {
        _startingValue = Value;
        
        base.OnThumbDragStarted(e);
    }

    protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
    {
        base.OnThumbDragCompleted(e);
        
        SelectedValue = Value;
        StartingValue = _startingValue;
    }
}