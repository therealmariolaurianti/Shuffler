using System.Windows.Controls;

namespace ShufflerPro.Upgraded.Objects;

public class SourceFolder : TreeViewItem
{
    public SourceFolder(string header)
    {
        Header = header;
        FullPath = string.Empty;
    }

    public SourceFolder(string name, string fullPath, bool isRoot)
    {
        Header = name;
        FullPath = fullPath;
        IsRoot = isRoot;

        ToolTip = FullPath;
    }

    public string FullPath { get; set; }
    public bool IsRoot { get; set; }
}