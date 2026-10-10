using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppRegistry.Web.Pages;

/// <summary>The home page has nothing of its own: it sends visitors straight to the Systems list.</summary>
public class IndexModel : PageModel
{
    public IActionResult OnGet() => RedirectToPage("/Systems/Index");
}
