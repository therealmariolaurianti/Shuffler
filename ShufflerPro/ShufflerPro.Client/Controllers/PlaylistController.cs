using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Controllers;

public class PlaylistController
{
    public void Initialize(Library library)
    {
        library.Playlists.Add(new Playlist
        {
            Name = "Favorites"
        });
    }
}