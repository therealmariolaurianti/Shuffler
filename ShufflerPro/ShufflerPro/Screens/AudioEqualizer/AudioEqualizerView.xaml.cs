using ShufflerPro.Framework.WPF.Windows;

namespace ShufflerPro.Screens.AudioEqualizer;

public partial class AudioEqualizerView
{
    public AudioEqualizerView()
    {
        InitializeComponent();
        WindowHelper.WindowsRoundCorners(GetWindow(this)!);
    }
}