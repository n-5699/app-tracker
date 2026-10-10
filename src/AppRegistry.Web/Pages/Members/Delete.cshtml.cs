using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Pages.Members;

/// <summary>Delete a member — only allowed when they aren't assigned to any system.</summary>
public class DeleteModel(AppDbContext db) : PageModel
{
    public Member Member { get; private set; } = default!;
    public int AssignedCount { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var member = await db.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (member is null)
            return NotFound();

        Member = member;
        AssignedCount = await db.Systems.SystemsFor(id).CountAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var member = await db.Members.FindAsync(id);
        if (member is null)
            return NotFound();

        // Re-check on submit: someone may have assigned them to a system in the meantime.
        if (await db.Systems.SystemsFor(id).AnyAsync())
            return RedirectToPage(new { id });

        db.Members.Remove(member);
        await db.SaveChangesAsync();
        TempData["Success"] = $"{member.FullName} deleted.";
        return RedirectToPage("Index");
    }
}
