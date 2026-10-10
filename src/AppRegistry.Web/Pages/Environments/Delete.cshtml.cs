using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Pages.Environments;

/// <summary>Confirm, then delete one environment. URL: /Environments/Delete/{id}</summary>
public class DeleteModel(AppDbContext db) : PageModel
{
    public AppEnvironment AppEnvironment { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var env = await db.Environments.Include(e => e.AppSystem).AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        if (env is null)
            return NotFound();

        AppEnvironment = env;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var env = await db.Environments.FindAsync(id);
        if (env is null)
            return NotFound();

        var systemId = env.AppSystemId;
        db.Environments.Remove(env);
        await db.SaveChangesAsync();
        return RedirectToPage("/Systems/Details", new { id = systemId });
    }
}
