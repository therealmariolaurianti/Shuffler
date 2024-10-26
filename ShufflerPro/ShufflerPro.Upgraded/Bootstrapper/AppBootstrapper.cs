using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using Caliburn.Micro;
using Ninject;
using ShufflerPro.Upgraded.Objects;
using ShufflerPro.Upgraded.Screens.Shell;

namespace ShufflerPro.Upgraded.Bootstrapper;

public class AppBootstrapper : BootstrapperBase
{
    private IKernel _kernel;

    public AppBootstrapper()
    {
        Initialize();
    }

    protected override void OnExit(object sender, EventArgs e)
    {
        _kernel.Dispose();
        base.OnExit(sender, e);
    }

    protected override object GetInstance(Type service, string key)
    {
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        return _kernel.Get(service);
    }

    protected override IEnumerable<object> GetAllInstances(Type service)
    {
        return _kernel.GetAll(service);
    }

    protected override void BuildUp(object instance)
    {
        _kernel.Inject(instance);
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
        _kernel = new StandardKernel();

        _kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
        _kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();

        DisplayRootViewForAsync<ShellViewModel>();
    }
}

public static class Extensions
{
    public static List<string> GetFilesByExtension(this string path, List<string> extensions)
    {
        return Directory
            .EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(s => extensions.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()))
            .ToList();
    }

    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
    {
        return new ObservableCollection<T>(items);
    }
}

public class Runner
{
    private readonly string _path = "X:\\";
    public List<Artist> Artists => Run();

    public List<Artist> Run()
    {
        var kernel = new StandardKernel();
        var albumFactory = kernel.Get<AlbumFactory>();
        var artistFactory = kernel.Get<ArtistFactory>();

        var folderPath = Path.GetFullPath(_path);

        var songs = folderPath.GetFilesByExtension(["mp3"]).Take(100);
        //var songFiles = songs.Select(file => new Song(file)).ToList();

        var songsx = new List<Song>();
        foreach (var song in songs)
        {
            try
            {
                var songFile = TagLib.File.Create(song);
                var newSong = new Song(songFile, song);
                songsx.Add(newSong);
            }
            catch (Exception e)
            {
                continue;
            }
        }
        
        
        
        var distinctSongs = songsx.Distinct().ToList();

        var songFilesWithArtists = distinctSongs.Where(s => s.Artist != null).ToList();
        var songFilesWithoutArtists = distinctSongs.Except(songFilesWithArtists).ToList();

        foreach (var songFilesWithoutArtist in songFilesWithoutArtists)
            songFilesWithoutArtist.UpdateArtist("Artist Unknown");

        songFilesWithArtists.AddRange(songFilesWithoutArtists);

        var albums = songFilesWithArtists.AsParallel().GroupBy(song => song.Album)
            .ToDictionary(s => s.Key, g => g.ToList());
        var albumList = new List<Album>();
        foreach (var kvp in albums)
        {
            var artist = kvp.Value.First().Artist;
            var album = albumFactory.Create(artist, kvp.Key, kvp.Value);
            albumList.Add(album);
        }

        var groupedAlbums = albumList.AsParallel().GroupBy(a => a.Artist).ToDictionary(s => s.Key, g => g.ToList());
        return groupedAlbums.Select(album => artistFactory.Create(album.Key, album.Value)).OrderBy(a => a.Name)
            .ToList();
    }
}