using System.Collections.ObjectModel;
using LiteDB;
using ShufflerPro.Database;

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

    public static T? TryGetIndex<T>(this List<T> items, int index)
    {
        try
        {
            return items[index];
        }
        catch (Exception)
        {
            return default;
        }
    }

    public static void AddRange<T>(this IEnumerable<T> items, ICollection<T> collection)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}