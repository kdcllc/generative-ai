using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ChatApp.Services;

public static partial class Utils
{
    public static string[] ExtractUrls(string input)
    {
        var regex = ExtractUrlsAttribute();
        var matches = regex.Matches(input);
        var urls = new string[matches.Count];
        for (int i = 0; i < matches.Count; i++)
        {
            urls[i] = matches[i].Value;
        }
        return urls;
    }

    public static string HashUrl(string url)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(url);
            var hash = sha256.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    [GeneratedRegex("\\b(?:https?://|www\\.)\\S+\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex ExtractUrlsAttribute();
}