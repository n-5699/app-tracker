using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Pages.Members;

/// <summary>Change a member's details, or deactivate them.</summary>
public class EditModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public Member Member { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var member = await db.Members.FindAsync(id);
        if (member is null)
            return NotFound();

        Member = member;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var existing = await db.Members.FindAsync(id);
        if (existing is null)
            return NotFound();

        Member.WorkEmail = Member.WorkEmail.Trim().ToLowerInvariant();
        if (await db.Members.EmailExistsAsync(Member.WorkEmail, exceptId: id))
            ModelState.AddModelError("Member.WorkEmail", "A member with this work email already exists.");

        // A member's role can't change while they fill that role on systems (it would break those assignments).
        if (Member.Role != existing.Role)
        {
            var assigned = await db.Systems.SystemsFor(id).CountAsync();
            if (assigned > 0)
                ModelState.AddModelError("Member.Role",
                    $"{existing.FullName} is the {existing.Role.DisplayName()} on {assigned} system(s). Assign someone else to those systems first.");
        }

        if (!ModelState.IsValid)
            return Page();

        existing.Role = Member.Role;
        existing.FullName = Member.FullName;
        existing.WorkEmail = Member.WorkEmail;
        existing.WorkPhone = Member.WorkPhone;
        existing.IsActive = Member.IsActive;

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("Member.WorkEmail", "A member with this work email already exists.");
            return Page();
        }

        TempData["Success"] = "Changes saved.";
        return RedirectToPage("Details", new { id });
    }
}
