using ShufflerPro.Client.Enums;

namespace ShufflerPro.Client.States;

public class RepeatState
{
    public RepeatState(bool isRepeatChecked, RepeatType repeatType)
    {
        IsRepeatChecked = isRepeatChecked;
        RepeatType = repeatType;
    }

    public bool IsRepeatChecked { get; }
    public RepeatType RepeatType { get; }
}