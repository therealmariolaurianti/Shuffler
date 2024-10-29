namespace ShufflerPro.Client.Entities;

public class SourceFolder
{
    public SourceFolder(string header, string toolTip)
    {
        Header = header;
        ToolTip = toolTip;
        FullPath = string.Empty;
        Items = new List<SourceFolder>();
    }

    public SourceFolder(string name, string fullPath, bool isRoot)
    {
        Header = name;
        FullPath = fullPath;
        IsRoot = isRoot;

        ToolTip = FullPath;
        Items = new List<SourceFolder>();
    }

    public string ToolTip { get; }
    public string Header { get; }
    public string FullPath { get; }
    public bool IsRoot { get; }
    public List<SourceFolder> Items { get; set; }
}