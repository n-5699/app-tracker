using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Pages.Environments;

/// <summary>Form to change an environment. URL: /Environments/Edit/{id}</summary>
public class EditModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public AppEnvironment AppEnvironment { get; set; } = default!;

    public AppSystem AppSystem { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var env = await db.Environments.Include(e => e.AppSystem).FirstOrDefaultAsync(e => e.Id == id);
        if (env is null)
            return NotFound();

        AppEnvironment = env;
        AppSystem = env.AppSystem!;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var existing = await db.Environments.Include(e => e.AppSystem).FirstOrDefaultAsync(e => e.Id == id);
        if (existing is null)
            return NotFound();

        AppSystem = existing.AppSystem!;
        ModelState.Remove("AppEnvironment.AppSystem");

        if (!ModelState.IsValid)
            return Page();

        // Copy only the editable fields (the environment can't be moved to another system).
        existing.Type = AppEnvironment.Type;
        existing.Label = AppEnvironment.Label;
        existing.Url = AppEnvironment.Url;
        existing.DeploymentLocation = AppEnvironment.DeploymentLocation;
        existing.Notes = AppEnvironment.Notes;

        await db.SaveChangesAsync();
        return RedirectToPage("/Systems/Details", new { id = existing.AppSystemId });
    }
}
