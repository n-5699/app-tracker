using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Pages.Members;

/// <summary>A member's contact details and every system they're involved in.</summary>
public class DetailsModel(AppDbContext db) : PageModel
{
    public Member Member { get; private set; } = default!;
    public List<AppSystem> Systems { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var member = await db.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (member is null)
            return NotFound();

        Member = member;
        Systems = await db.Systems.SystemsFor(id)
            .Include(s => s.Environments)
            .OrderBy(s => s.Name)
            .AsNoTracking()
            .ToListAsync();
        return Page();
    }
}
