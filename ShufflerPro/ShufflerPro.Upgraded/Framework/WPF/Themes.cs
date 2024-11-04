using ShufflerPro.Client.Entities;

namespace ShufflerPro.Upgraded.Framework.WPF;

public static class Themes
{
    public static IEnumerable<Theme> Items => new[]
    {
        new Theme("Red"),
        new Theme("Green"),
        new Theme("Blue"),
        new Theme("Purple"),
        new Theme("Orange"),
        new Theme("Lime"),
        new Theme("Emerald"),
        new Theme("Teal"),
        new Theme("Cyan"),
        new Theme("Cobalt"),
        new Theme("Indigo"),
        new Theme("Violet"),
        new Theme("Pink"),
        new Theme("Magenta"),
        new Theme("Crimson"),
        new Theme("Amber"),
        new Theme("Yellow"),
        new Theme("Brown"),
        new Theme("Olive"),
        Default,
        new Theme("Mauve"),
        new Theme("Taupe"),
        new Theme("Sienna")
    }.OrderBy(d => d.Name);
    
    public static readonly Theme Default = new("Steel");
}