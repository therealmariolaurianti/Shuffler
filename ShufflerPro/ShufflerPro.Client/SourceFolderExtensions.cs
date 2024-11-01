using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client;

public static class SourceFolderExtensions
{
    public static IEnumerable<SourceFolder> Flatten(this SourceFolder node)
    {
        var nodes = new Stack<SourceFolder>();
        nodes.Push(node);

        while (nodes.Count > 0)
        {
            var n = nodes.Pop();
            yield return n;

            for (var i = n.Items.Count - 1; i >= 0; i--)
                nodes.Push(n.Items[i]);
        }
    }
}