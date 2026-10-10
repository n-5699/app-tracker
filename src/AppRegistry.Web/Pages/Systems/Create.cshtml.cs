using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Pages.Systems;

/// <summary>Form to add a new system.</summary>
public class CreateModel(AppDbContext db) : PageModel
{
    // [BindProperty] fills this object from the submitted form fields.
    [BindProperty]
    public AppSystem AppSystem { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        // Environments are added on their own pages, so don't validate them here.
        ModelState.Remove("AppSystem.Environments");

        AppSystem.Name = AppSystem.Name.Trim();
        if (await db.Systems.NameExistsAsync(AppSystem.Name))
            ModelState.AddModelError("AppSystem.Name", "A system with this name already exists.");

        if (!ModelState.IsValid)
            return Page();   // show the form again with error messages

        db.Systems.Add(AppSystem);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Someone else saved the same name a moment ago (the database's unique index caught it).
            ModelState.AddModelError("AppSystem.Name", "A system with this name already exists.");
            return Page();
        }
        TempData["Success"] = $"System \"{AppSystem.Name}\" created.";
        return RedirectToPage("Details", new { id = AppSystem.Id });
    }
}
