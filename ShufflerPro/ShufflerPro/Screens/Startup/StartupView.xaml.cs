using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ShufflerPro.Screens.Startup;

public partial class StartupView
{
    public StartupView()
    {
        InitializeComponent();
        TypewriteTextblock("Calculating....", LoadingTextBlock, TimeSpan.FromSeconds(5));
    }

    private void TypewriteTextblock(string textToAnimate, TextBlock txt, TimeSpan timeSpan)
    {
        var story = new Storyboard
        {
            FillBehavior = FillBehavior.HoldEnd,
            RepeatBehavior = RepeatBehavior.Forever
        };

        var stringAnimationUsingKeyFrames = new StringAnimationUsingKeyFrames
        {
            Duration = new Duration(timeSpan)
        };

        var tmp = string.Empty;
        foreach (var c in textToAnimate)
        {
            var discreteStringKeyFrame = new DiscreteStringKeyFrame
            {
                KeyTime = KeyTime.Paced
            };
            
            tmp += c;
            discreteStringKeyFrame.Value = tmp;
            stringAnimationUsingKeyFrames.KeyFrames.Add(discreteStringKeyFrame);
        }

        Storyboard.SetTargetName(stringAnimationUsingKeyFrames, txt.Name);
        Storyboard.SetTargetProperty(stringAnimationUsingKeyFrames, new PropertyPath(TextBlock.TextProperty));
        
        story.Children.Add(stringAnimationUsingKeyFrames);
        story.Begin(txt);
    }
}