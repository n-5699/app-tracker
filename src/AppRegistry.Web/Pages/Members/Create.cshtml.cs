using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Pages.Members;

/// <summary>Form to add a member. Optional ?role=SystemAdmin pre-selects the role.</summary>
public class CreateModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public Member Member { get; set; } = new();

    public void OnGet(MemberRole? role)
    {
        if (role is not null)
            Member.Role = role.Value;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Member.WorkEmail = Member.WorkEmail.Trim().ToLowerInvariant();
        if (await db.Members.EmailExistsAsync(Member.WorkEmail))
            ModelState.AddModelError("Member.WorkEmail", "A member with this work email already exists.");

        if (!ModelState.IsValid)
            return Page();

        Member.IsActive = true;
        db.Members.Add(Member);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("Member.WorkEmail", "A member with this work email already exists.");
            return Page();
        }

        TempData["Success"] = $"{Member.FullName} added as {Member.Role.DisplayName()}.";
        return RedirectToPage("Index");
    }
}
