using System.Windows.Controls;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Framework.WPF;

public class SourceTreeViewItem : TreeViewItem
{
    public SourceFolder SourceFolder { get; set; }
}