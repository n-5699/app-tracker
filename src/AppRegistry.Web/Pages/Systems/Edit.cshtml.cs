using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Pages.Systems;

/// <summary>Form to change an existing system's details.</summary>
public class EditModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public AppSystem AppSystem { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var system = await db.Systems.FindAsync(id);
        if (system is null)
            return NotFound();

        AppSystem = system;
        await LoadMemberOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        ModelState.Remove("AppSystem.Environments");

        var existing = await db.Systems.FindAsync(id);
        if (existing is null)
            return NotFound();

        AppSystem.Name = AppSystem.Name.Trim();
        if (await db.Systems.NameExistsAsync(AppSystem.Name, exceptId: id))
            ModelState.AddModelError("AppSystem.Name", "A system with this name already exists.");

        await db.ValidateAssignmentsAsync(AppSystem, ModelState, existing);

        if (!ModelState.IsValid)
        {
            await LoadMemberOptionsAsync();
            return Page();
        }

        // Copy only the editable fields onto the saved record (Id and CreatedAt stay as they were).
        existing.Name = AppSystem.Name;
        existing.Description = AppSystem.Description;
        existing.BusinessOwnerId = AppSystem.BusinessOwnerId;
        existing.SystemAdminId = AppSystem.SystemAdminId;
        existing.ItDeveloperId = AppSystem.ItDeveloperId;

        try
        {
            await db.SaveChangesAsync();   // UpdatedAt is set automatically in AppDbContext
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("AppSystem.Name", "A system with this name already exists.");
            await LoadMemberOptionsAsync();
            return Page();
        }
        TempData["Success"] = "Changes saved.";
        return RedirectToPage("Details", new { id });
    }

    /// <summary>Fills the three people dropdowns with members of the matching role.</summary>
    private async Task LoadMemberOptionsAsync()
    {
        ViewData["BusinessOwnerIdOptions"] = await db.MemberOptionsAsync(MemberRole.BusinessOwner, AppSystem.BusinessOwnerId);
        ViewData["SystemAdminIdOptions"] = await db.MemberOptionsAsync(MemberRole.SystemAdmin, AppSystem.SystemAdminId);
        ViewData["ItDeveloperIdOptions"] = await db.MemberOptionsAsync(MemberRole.ItDeveloper, AppSystem.ItDeveloperId);
    }
}
