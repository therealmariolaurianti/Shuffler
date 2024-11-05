using System.Collections.ObjectModel;

namespace ShufflerPro.Client;

public static class Extensions
{
    public static List<string> DefaultExtensions => ["mp3", "m4a"];

    public static List<string> GetFilesByExtension(this string path)
    {
        return Directory
            .EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly)
            .Where(s => DefaultExtensions.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()))
            .ToList();
    }

    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
    {
        return new ObservableCollection<T>(items);
    }

    public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> items)
    {
        return new ReadOnlyCollection<T>(new List<T>(items));
    }

    public static TimeSpan StripMilliseconds(this TimeSpan time)
    {
        return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);
    }

    public static double Reset(this double value)
    {
        return 0d;
    }

    public static string DefaultTimeSpan(this string timeSpanDisplay)
    {
        return new TimeSpan().ToString("mm':'ss");
    }
}