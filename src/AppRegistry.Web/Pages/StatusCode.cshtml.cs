using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppRegistry.Web.Pages;

/// <summary>Friendly page for error status codes (404 Not Found, etc.). Wired up in Program.cs.</summary>
[IgnoreAntiforgeryToken]
public class StatusCodeModel : PageModel
{
    public int Code { get; private set; }

    public string Heading => Code switch
    {
        404 => "Page not found",
        400 => "That request didn't work",
        _ => "Something went wrong"
    };

    public string Explanation => Code switch
    {
        404 => "The system or environment may have been deleted, or the link is incorrect.",
        400 => "The form may have expired. Go back, refresh the page, and try again.",
        _ => "Please try again. If it keeps happening, let the App Registry maintainer know."
    };

    public void OnGet(int code) => Code = code;
}
