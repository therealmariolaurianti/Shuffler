namespace ShufflerPro.Client.Entities;

public class SourceFolder
{
    public SourceFolder(string header)
    {
        Header = BuildHeader(header);
        FullPath = string.Empty;
        IsRoot = false;
        Items = new List<SourceFolder>();
    }

    public SourceFolder(string name, string fullPath, bool isRoot)
    {
        Header = BuildHeader(name);
        FullPath = fullPath;
        IsRoot = isRoot;
        Items = new List<SourceFolder>();
    }

    public string Header { get; }
    public string FullPath { get; }
    public bool IsRoot { get; }
    public List<SourceFolder> Items { get; set; }

    private static string BuildHeader(string header)
    {
        return header.Replace(Path.DirectorySeparatorChar, ' ').Trim();
    }
}