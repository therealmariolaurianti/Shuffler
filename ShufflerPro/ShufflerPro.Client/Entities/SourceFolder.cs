namespace ShufflerPro.Client.Entities;

public class SourceFolder
{
    public SourceFolder(string header, SourceFolder parent, string fullPath)
    {
        Header = BuildHeader(header);
        FullPath = fullPath;
        IsRoot = false;
        Parent = parent;
        Items = new List<SourceFolder>();
    }

    public SourceFolder(string name, string fullPath, bool isRoot, SourceFolder? parent)
    {
        Header = BuildHeader(name);
        FullPath = fullPath;
        IsRoot = isRoot;
        Parent = parent;
        Items = new List<SourceFolder>();
    }

    public string Header { get; }
    public string FullPath { get; }
    public bool IsRoot { get; }
    public List<SourceFolder> Items { get; set; }
    public SourceFolder? Parent { get; set; }
    public bool IsProcessed { get; set; }

    private static string BuildHeader(string header)
    {
        return header.Replace(Path.DirectorySeparatorChar, ' ').Trim();
    }
}