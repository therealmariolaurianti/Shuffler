using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Extensions;

public static class LinqExtensions
{
    public static List<string> DefaultExtensions => ["mp3", "m4a", "flac"];

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

    public static void AddRange<T>(this ICollection<T> items, IEnumerable<T> collection)
    {
        foreach (var item in collection) items.Add(item);
    }

    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(_ => Guid.NewGuid());
    }

    public static string? ToFormattedString(this string[]? items, string separator)
    {
        return items is null ? null : string.Join(separator, items);
    }

    public static TimeSpan Tick(this TimeSpan timeSpan)
    {
        return timeSpan.Add(new TimeSpan(0, 0, 1));
    }
}