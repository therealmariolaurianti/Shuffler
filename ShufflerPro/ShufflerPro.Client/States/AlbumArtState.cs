namespace ShufflerPro.Client.States;

public class AlbumArtState
{
    public AlbumArtState(byte[]? albumArt, bool albumArtChanged)
    {
        AlbumArt = albumArt;
        AlbumArtChanged = albumArtChanged;
    }

    public byte[]? AlbumArt { get; }
    public bool AlbumArtChanged { get; }
}