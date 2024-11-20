using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Extensions;

namespace ShufflerPro.Controllers;

public class SourceTreeState(List<Song> songs)
{
    public ObservableCollection<Song> Songs => songs.ToObservableCollection();
}