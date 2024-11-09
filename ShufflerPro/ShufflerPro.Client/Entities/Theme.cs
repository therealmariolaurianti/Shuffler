namespace ShufflerPro.Client.Entities;

public class Theme(string id, string name)
{
    public string Name { get; set; } = name;
    public Guid Id = Guid.Parse(id);
}