﻿using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class SourceFolderController
{
    public NewResult<ObservableCollection<SourceFolder>> BuildSourceFolders(string folderPath,
        ICollection<SourceFolder> existingSourceFolders)
    {
        return FindRoot(folderPath, existingSourceFolders)
            .Map(rootFolder =>
            {
                var allFolders = new List<SourceFolder> { rootFolder };
                FindAllFolders(rootFolder, allFolders);

                var levels = folderPath.Split(Path.DirectorySeparatorChar).ToList();

                Build(rootFolder.Header, levels, rootFolder, allFolders, folderPath);

                return existingSourceFolders.ToObservableCollection();
            });
    }

    private static void FindAllFolders(SourceFolder rootFolder, List<SourceFolder> allFolders)
    {
        foreach (var rootFolderItem in rootFolder.Items)
        {
            allFolders.Add(rootFolderItem);
            FindAllFolders(rootFolderItem, allFolders);
        }
    }

    private static void Build(string root, List<string> levels, SourceFolder rootFolder,
        List<SourceFolder> allFolders, string fullPath)
    {
        var lastLevel = levels.Last();

        foreach (var level in levels)
        {
            var index = levels.IndexOf(level);
            var currentPath = string.Join(Path.DirectorySeparatorChar, levels.Take(index + 1));
            
            if (level == root)
                continue;
            
            var items = allFolders.SingleOrDefault(af => af.Header == level && af.FullPath == currentPath);
            if (items is not null)
            {
                rootFolder = items;
                continue;
            }
            
            var item = new SourceFolder(level, rootFolder, currentPath);
            var existingFolder = allFolders
                .SingleOrDefault(f => f.Header == item.Header && f.Parent?.Header == item.Parent?.Header);
            if (existingFolder is not null)
            {
                rootFolder = existingFolder;
            }
            else
            {
                rootFolder.Items.Add(item);
                rootFolder = item;
                allFolders.Add(item);
            }

            if (lastLevel == level)
                BuildChildDirectories(root, rootFolder, allFolders, fullPath);
        }
    }

    private static void BuildChildDirectories(string root, SourceFolder rootFolder, List<SourceFolder> allFolders,
        string fullPath)
    {
        try
        {
            var directories = Directory.GetDirectories(fullPath);
            if (!directories.Any())
                return;
            foreach (var directory in directories)
            {
                var x = directory.Split(Path.DirectorySeparatorChar).ToList();
                Build(root, x, rootFolder, allFolders, directory);
            }
        }
        catch (Exception)
        {
            //ignore
        }
    }

    private NewResult<SourceFolder> FindRoot(string folderPath, ICollection<SourceFolder> existingSourceFolders)
    {
        var rootPath = Path.GetPathRoot((string?)folderPath)?.Replace(Path.DirectorySeparatorChar, ' ').Trim();
        if (rootPath is null)
            return new Exception("Failed to load source");

        var root = existingSourceFolders.SingleOrDefault(sf => sf.IsRoot && sf.Header == rootPath);
        return root ?? BuildRoot(rootPath, existingSourceFolders);
    }

    private NewResult<SourceFolder> BuildRoot(string rootPath, ICollection<SourceFolder> existingSourceFolders)
    {
        var root = new SourceFolder(rootPath, rootPath, true, null);
        existingSourceFolders.Add(root);
        return root;
    }
}