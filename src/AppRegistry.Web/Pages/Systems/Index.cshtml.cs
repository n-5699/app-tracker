using AppRegistry.Web.Data;
using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Pages.Systems;

/// <summary>Lists systems alphabetically, with optional search and environment-type filter.</summary>
public class IndexModel(AppDbContext db) : PageModel
{
    public List<AppSystem> Systems { get; private set; } = new();

    /// <summary>Total number of systems before searching/filtering.</summary>
    public int TotalCount { get; private set; }

    /// <summary>Search text, from the URL: /Systems?q=payroll</summary>
    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    /// <summary>Only show systems that have this environment type: /Systems?type=UAT</summary>
    [BindProperty(SupportsGet = true)]
    public EnvironmentType? Type { get; set; }

    public bool IsFiltered => !string.IsNullOrWhiteSpace(Q) || Type is not null;

    public async Task OnGetAsync()
    {
        TotalCount = await db.Systems.CountAsync();

        IQueryable<AppSystem> query = db.Systems
            .Include(s => s.Environments)
            .Include(s => s.BusinessOwner)
            .Include(s => s.SystemAdmin)
            .Include(s => s.ItDeveloper);

        if (!string.IsNullOrWhiteSpace(Q))
        {
            // LIKE '%text%' — case-insensitive in both SQLite and Azure SQL.
            // Escape % and _ so they're searched for literally.
            var text = Q.Trim().Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");
            var pattern = $"%{text}%";

            query = query.Where(s =>
                EF.Functions.Like(s.Name, pattern, @"\") ||
                EF.Functions.Like(s.Description ?? "", pattern, @"\") ||
                // the assigned people's names and emails
                (s.BusinessOwner != null && (EF.Functions.Like(s.BusinessOwner.FullName, pattern, @"\") || EF.Functions.Like(s.BusinessOwner.WorkEmail, pattern, @"\"))) ||
                (s.SystemAdmin != null && (EF.Functions.Like(s.SystemAdmin.FullName, pattern, @"\") || EF.Functions.Like(s.SystemAdmin.WorkEmail, pattern, @"\"))) ||
                (s.ItDeveloper != null && (EF.Functions.Like(s.ItDeveloper.FullName, pattern, @"\") || EF.Functions.Like(s.ItDeveloper.WorkEmail, pattern, @"\"))) ||
                // also find systems by an environment's URL or location, e.g. a server name
                s.Environments.Any(e =>
                    EF.Functions.Like(e.Url, pattern, @"\") ||
                    EF.Functions.Like(e.DeploymentLocation, pattern, @"\")));
        }

        if (Type is not null)
            query = query.Where(s => s.Environments.Any(e => e.Type == Type));

        Systems = await query
            .OrderBy(s => s.Name)
            .AsNoTracking()
            .ToListAsync();
    }
}
