using System.Text.RegularExpressions;

namespace ChatApp.Services;

public static class UrlExtractor
{
    public static string[] ExtractUrls(string input)
    {
        var regex = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        var matches = regex.Matches(input);
        var urls = new string[matches.Count];
        for (int i = 0; i < matches.Count; i++)
        {
            urls[i] = matches[i].Value;
        }
        return urls;
    }
}