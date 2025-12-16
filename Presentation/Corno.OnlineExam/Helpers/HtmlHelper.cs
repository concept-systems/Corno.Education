using System.Text.RegularExpressions;
using System.Web;

// For HtmlDecode (available in .NET Framework)
// For .NET Core: use System.Net.WebUtility.HtmlDecode instead

namespace Corno.OnlineExam.Helpers;

public static class HtmlHelper
{
    /// <summary>
    /// Converts an HTML string (encoded or raw) to plain text.
    /// </summary>
    /// <param name="html">HTML string (e.g., "&lt;p&gt;Hello&lt;/p&gt;" or "<p>Hello</p>")</param>
    /// <returns>Plain text without tags or entities.</returns>
    public static string GetPlainText(this string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        // Decode HTML entities first
        var decoded = HttpUtility.HtmlDecode(html);
        // For .NET Core: use WebUtility.HtmlDecode(html);

        // Remove all tags
        var plainText = Regex.Replace(decoded, "<.*?>", string.Empty);

        // Trim extra spaces/newlines
        return plainText.Trim();
    }
}