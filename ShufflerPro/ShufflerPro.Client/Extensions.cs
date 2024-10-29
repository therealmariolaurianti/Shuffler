﻿using System.Collections.ObjectModel;

namespace ShufflerPro.Client;

public static class Extensions
{
    public static List<string> DefaultExtensions => ["mp3", ".m4a"];

    public static List<string> GetFilesByExtension(this string path, List<string> extensions)
    {
        return Directory
            .EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly)
            .Where(s => extensions.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()))
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
}