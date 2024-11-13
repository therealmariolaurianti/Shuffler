using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;

namespace ShufflerPro.Framework;

public class MusicPlayerCommands
{
    public MusicPlayerCommands(Action playPause, Action mute)
    {
        PlayPauseCommand = new ActionCommand(playPause);
        MuteCommand = new ActionCommand(mute);
    }

    public ICommand PlayPauseCommand { get; internal set; }
    public ICommand MuteCommand { get; internal set; }
}