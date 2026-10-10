using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppRegistry.Web.Helpers;

public static class LocalTimeHtmlHelper
{
    /// <summary>
    /// Renders a UTC time as &lt;time datetime="...Z"&gt;. site.js rewrites it in the viewer's own time zone
    /// (the server may be in UTC on Azure, so converting on the server would show the wrong hour).
    /// </summary>
    public static IHtmlContent LocalTime(this IHtmlHelper html, DateTime utc, bool includeTime = false)
    {
        utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);   // SQLite returns "Unspecified"; we always store UTC

        var tag = new TagBuilder("time");
        tag.Attributes["datetime"] = utc.ToString("yyyy-MM-ddTHH:mm:ssZ");
        tag.Attributes["data-local"] = includeTime ? "datetime" : "date";
        tag.Attributes["title"] = utc.ToString("yyyy-MM-dd HH:mm") + " UTC";
        // Fallback text if JavaScript is off: the UTC time, labelled.
        tag.InnerHtml.Append(includeTime ? utc.ToString("MMM d, yyyy h:mm tt") + " UTC" : utc.ToString("MMM d, yyyy"));
        return tag;
    }
}
