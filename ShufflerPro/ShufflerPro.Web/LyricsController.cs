using System.Text.RegularExpressions;
using GeniusLyrics.NET;
using GeniusLyrics.NET.Models.SearchSong;
using ShufflerPro.Client;
using ShufflerPro.Result;

namespace ShufflerPro.Web;

public class LyricsController(AccessKeysContainer accessKeysContainer)
{
    public async Task<NewResult<string>> Load(string artist, string title)
    {
        if (accessKeysContainer.GeniusToken is null)
            return NewResultExtensions.CreateFail<string>("Access token not authenticated");
        return await LoadLyrics(title, artist);
    }

    private async Task<NewResult<string>> LoadLyrics(string artist, string title)
    {
        var genius = new GeniusClient(accessKeysContainer.GeniusToken!);

        try
        {
            var song = await genius.GetSong(title, artist, true);

            return Format(song) ?? NewResultExtensions.CreateFail<string>(new Exception("Error fetching song data."));
        }
        catch (Exception e)
        {
            return NewResultExtensions.CreateFail<string>(e);
        }
    }

    private static string? Format(Song? song)
    {
        if (song?.Lyrics is null)
            return null;

        var rawLyrics = Regex.Replace(song.Lyrics, @"(\[.*?\])", Environment.NewLine + "$1" + Environment.NewLine);
        return rawLyrics.Trim();
    }
}