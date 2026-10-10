using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Pages.Systems;

/// <summary>Confirm, then delete a system and (by cascade) all its environments.</summary>
public class DeleteModel(AppDbContext db) : PageModel
{
    public AppSystem AppSystem { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var system = await db.Systems
            .Include(s => s.Environments)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (system is null)
            return NotFound();

        AppSystem = system;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var system = await db.Systems.FindAsync(id);
        if (system is not null)
        {
            db.Systems.Remove(system);
            await db.SaveChangesAsync();
        }
        return RedirectToPage("Index");
    }
}
