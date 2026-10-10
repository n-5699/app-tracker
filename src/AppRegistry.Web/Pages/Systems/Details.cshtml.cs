using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Pages.Systems;

/// <summary>Everything about one system, including its environments.</summary>
public class DetailsModel(AppDbContext db) : PageModel
{
    public AppSystem AppSystem { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var system = await db.Systems
            .Include(s => s.Environments)
            .Include(s => s.BusinessOwner)
            .Include(s => s.SystemAdmin)
            .Include(s => s.ItDeveloper)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (system is null)
            return NotFound();

        AppSystem = system;
        return Page();
    }
}
