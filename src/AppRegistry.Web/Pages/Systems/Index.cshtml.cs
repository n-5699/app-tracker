using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Pages.Systems;

/// <summary>Lists every system, alphabetically.</summary>
public class IndexModel(AppDbContext db) : PageModel
{
    public List<AppSystem> Systems { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Systems = await db.Systems
            .Include(s => s.Environments)   // load environments too, so we can count them
            .OrderBy(s => s.Name)
            .AsNoTracking()                 // read-only: faster, nothing will be saved
            .ToListAsync();
    }
}
