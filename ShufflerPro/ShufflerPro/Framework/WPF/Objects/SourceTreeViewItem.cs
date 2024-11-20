using System.Windows.Controls;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Framework.WPF.Objects;

public class SourceTreeViewItem : TreeViewItem
{
    public SourceFolder SourceFolder { get; set; }
    public Guid Id { get; set; }
    public bool IsTopLevelItem { get; set; }
}