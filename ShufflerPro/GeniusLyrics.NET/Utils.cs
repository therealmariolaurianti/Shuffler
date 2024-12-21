using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace GeniusLyrics.NET;

internal static class Utils
{
    public static string GetTitle(string title, string artist)
    {
        var combined = $"{title} {artist}".ToLower();

        // Remove content within parentheses
        combined = Regex.Replace(combined, @" *\([^)]*\) *", "");

        // Remove content within square brackets
        combined = Regex.Replace(combined, @" *\[[^\]]*\] *", "");

        // Remove "feat." or "ft."
        combined = Regex.Replace(combined, @"feat\.|ft\.", "");

        // Replace multiple spaces with a single space
        combined = Regex.Replace(combined, @"\s+", " ").Trim();

        return combined;
    }

    public static async Task<string?> ExtractLyrics(string url)
    {
        HtmlWeb web = new();

        var htmlDoc = await web.LoadFromWebAsync(url);
        
        var nodes = htmlDoc.DocumentNode.SelectNodes("//div[class='Lyrics']") ??
                    htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'Lyrics__Container')]") ??
                    htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'Lyrics-sc')]");

        if (nodes == null)
            return null;
        var lyrics = "";
        foreach (var node in nodes)
        {
            var textOnly = node.SelectNodes(".//text()");

            if (textOnly == null)
                continue;

            foreach (var textNode in textOnly)
                lyrics += HtmlEntity.DeEntitize(textNode.InnerText).Trim() + "\n";
        }

        return lyrics.Trim();
    }
}