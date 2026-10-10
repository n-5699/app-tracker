using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Pages.Members;

/// <summary>Lists members (active by default), optionally filtered by role.</summary>
public class IndexModel(AppDbContext db) : PageModel
{
    public List<Member> Members { get; private set; } = new();

    /// <summary>How many systems each member is assigned to, by member Id.</summary>
    public Dictionary<int, int> SystemCounts { get; private set; } = new();

    public int TotalCount { get; private set; }
    public int InactiveCount { get; private set; }

    /// <summary>/Members?role=SystemAdmin</summary>
    [BindProperty(SupportsGet = true)]
    public MemberRole? Role { get; set; }

    /// <summary>/Members?inactive=true also lists deactivated members.</summary>
    [BindProperty(SupportsGet = true)]
    public bool Inactive { get; set; }

    public async Task OnGetAsync()
    {
        TotalCount = await db.Members.CountAsync();
        InactiveCount = await db.Members.CountAsync(m => !m.IsActive);

        var query = db.Members.AsNoTracking();
        if (!Inactive)
            query = query.Where(m => m.IsActive);
        if (Role is not null)
            query = query.Where(m => m.Role == Role);

        Members = await query.OrderBy(m => m.FullName).ToListAsync();
        SystemCounts = await db.MemberSystemCountsAsync();
    }
}
