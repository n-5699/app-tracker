using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppRegistry.Web.Pages.Environments;

/// <summary>Form to add an environment to a system. URL: /Environments/Create/{systemId}</summary>
public class CreateModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public AppEnvironment AppEnvironment { get; set; } = new();

    /// <summary>The system this environment is being added to (shown in the page heading).</summary>
    public AppSystem AppSystem { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(int systemId)
    {
        var system = await db.Systems.FindAsync(systemId);
        if (system is null)
            return NotFound();

        AppSystem = system;
        AppEnvironment.Type = EnvironmentType.Production;   // most common choice
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int systemId)
    {
        var system = await db.Systems.FindAsync(systemId);
        if (system is null)
            return NotFound();

        AppSystem = system;
        ModelState.Remove("AppEnvironment.AppSystem");   // set from the URL, not the form

        if (!ModelState.IsValid)
            return Page();

        AppEnvironment.AppSystemId = systemId;
        db.Environments.Add(AppEnvironment);
        await db.SaveChangesAsync();
        return RedirectToPage("/Systems/Details", new { id = systemId });
    }
}
