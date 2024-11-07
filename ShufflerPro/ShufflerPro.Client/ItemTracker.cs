using System.Reflection;

namespace ShufflerPro.Client;

public class ItemTracker<T>
{
    private readonly Dictionary<string, object?> _snapshot = [];

    public bool IsDirty => PropertyDifferences.Count != 0;

    public T Item { get; set; } = default!;
    public Dictionary<string, object?> PropertyDifferences => GetPropertyDifferences();

    private Dictionary<string, object?> GetPropertyDifferences()
    {
        var propertyDifferences = _snapshot
            .Where(kv => !Equals(kv.Value, Item!.GetType().GetProperty(kv.Key)?.GetValue(Item)))
            .ToDictionary(kv => kv.Key, kv => Item!.GetType().GetProperty(kv.Key)?.GetValue(Item));
        return propertyDifferences;
    }

    public void Attach(T item)
    {
        Item = item;
        TakeSnapshot();
    }

    private void TakeSnapshot()
    {
        var props = GetPropertyInfos();

        foreach (var prop in props)
        {
            var propValue = prop.GetValue(Item, null);
            _snapshot[prop.Name] = propValue;
        }
    }

    public void Revert()
    {
        var props = GetPropertyInfos();

        foreach (var prop in props) prop.SetValue(Item, _snapshot[prop.Name]);
    }

    private List<PropertyInfo> GetPropertyInfos()
    {
        var propertyInfos = Item!.GetType().GetProperties().Where(d => d.CanWrite);
        var props = new List<PropertyInfo>(propertyInfos);
        return props;
    }
}