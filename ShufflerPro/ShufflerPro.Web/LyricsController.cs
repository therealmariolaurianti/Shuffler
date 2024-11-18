using System.Text.RegularExpressions;
using GeniusLyrics.NET;
using GeniusLyrics.NET.Models.SearchSong;
using ShufflerPro.Client;
using ShufflerPro.Result;

namespace ShufflerPro.Web;

public class LyricsController
{
    private readonly AccessKeysContainer _accessKeysContainer;

    public LyricsController(AccessKeysContainer accessKeysContainer)
    {
        _accessKeysContainer = accessKeysContainer;
    }

    public async Task<NewResult<string>> Load(string artist, string title)
    {
        if (_accessKeysContainer.GeniusToken is null)
            return NewResultExtensions.CreateFail<string>("Access token not authenticated");
        return await LoadLyrics(title, artist);
    }

    private async Task<NewResult<string>> LoadLyrics(string artist, string title)
    {
        var genius = new GeniusClient(_accessKeysContainer.GeniusToken!);
        var song = await genius.GetSong(title, artist, true);

        return Format(song) ?? NewResultExtensions.CreateFail<string>(new Exception("Error fetching song data."));
    }

    private static string? Format(Song? song)
    {
        if (song?.Lyrics is null)
            return null;

        var rawLyrics = Regex.Replace(song.Lyrics, @"(\[.*?\])", Environment.NewLine + "$1" + Environment.NewLine);
        return rawLyrics.Trim();
    }
}