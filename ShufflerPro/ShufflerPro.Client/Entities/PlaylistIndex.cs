namespace ShufflerPro.Client.Entities;

public class PlaylistIndex(Guid id, int index)
{
    public Guid Id { get; } = id;
    public int Index { get; } = index;
}