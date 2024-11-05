using System.ComponentModel;

namespace ShufflerPro.Framework;

public sealed class NotifyTaskCompletion : INotifyPropertyChanged
{
    private readonly Action? _complete;
    private readonly Action? _done;
    private readonly Action<Exception>? _error;
    private readonly Func<Task>? _taskFunc;

    public NotifyTaskCompletion(Task task, Action? complete = null, Action<Exception>? error = null,
        Action? done = null)
    {
        _complete = complete;
        _error = error;
        _done = done;
        Task = task;

        if (!task.IsCompleted) _ = WatchTaskAsync(Task);
    }

    public NotifyTaskCompletion(Func<Task> taskFunc, Action? complete = null, Action<Exception>? error = null,
        Action? done = null)
    {
        _complete = complete;
        _error = error;
        _done = done;
        _taskFunc = taskFunc;
    }

    public Task? Task { get; private set; }
    public TaskStatus Status => Task?.Status ?? TaskStatus.WaitingForActivation;
    public bool IsCompleted => Task?.IsCompleted ?? false;
    public bool IsNotCompleted => !IsCompleted == false;
    public bool IsSuccessfullyCompleted => Task?.Status == TaskStatus.RanToCompletion;
    public bool IsCanceled => Task?.IsCanceled ?? false;
    public bool IsFaulted => Task?.IsFaulted ?? false;
    public AggregateException? Exception => Task?.Exception;
    public Exception? InnerException => Exception?.InnerException;
    public string? ErrorMessage => InnerException?.Message;
    public event PropertyChangedEventHandler? PropertyChanged;

    public void StartTask()
    {
        if (Task == null || !Task.IsCompleted)
        {
            Task = _taskFunc?.Invoke();

            if (Task != null) 
                _ = WatchTaskAsync(Task);
        }
    }

    private async Task WatchTaskAsync(Task task)
    {
        try
        {
            await task;
            _complete?.Invoke();
            _done?.Invoke();
        }
        catch (Exception ex)
        {
            _error?.Invoke(ex);
            _done?.Invoke();
        }

        var propertyChanged = PropertyChanged;
        if (propertyChanged == null)
            return;
        propertyChanged(this, new PropertyChangedEventArgs("Status"));
        propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
        propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
        if (task.IsCanceled)
        {
            propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
        }
        else if (task.IsFaulted)
        {
            propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
            propertyChanged(this, new PropertyChangedEventArgs("Exception"));
            propertyChanged(this, new PropertyChangedEventArgs("InnerException"));
            propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
        }
        else
        {
            propertyChanged(this, new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("Result"));
        }
    }

    public void Refresh()
    {
        Task = null;
        StartTask();
    }
}