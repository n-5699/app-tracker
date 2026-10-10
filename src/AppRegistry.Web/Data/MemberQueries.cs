using AppRegistry.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Data;

/// <summary>Reusable member-related queries and checks, shared by the Members and Systems pages.</summary>
public static class MemberQueries
{
    /// <summary>True if another member already uses this work email (emails are stored lower-case).</summary>
    public static Task<bool> EmailExistsAsync(this IQueryable<Member> members, string email, int? exceptId = null)
    {
        var lower = email.Trim().ToLowerInvariant();
        return members.AnyAsync(m => m.WorkEmail == lower && m.Id != exceptId);
    }

    /// <summary>Every system the member is assigned to, in any role.</summary>
    public static IQueryable<AppSystem> SystemsFor(this IQueryable<AppSystem> systems, int memberId) =>
        systems.Where(s => s.BusinessOwnerId == memberId || s.SystemAdminId == memberId || s.ItDeveloperId == memberId);

    /// <summary>Number of systems each member is assigned to, keyed by member Id.</summary>
    public static async Task<Dictionary<int, int>> MemberSystemCountsAsync(this AppDbContext db)
    {
        var assignments = await db.Systems
            .Select(s => new { s.BusinessOwnerId, s.SystemAdminId, s.ItDeveloperId })
            .ToListAsync();

        return assignments
            .SelectMany(a => new[] { a.BusinessOwnerId, a.SystemAdminId, a.ItDeveloperId }.Where(id => id is not null).Distinct())
            .GroupBy(id => id!.Value)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// Dropdown choices for one role: active members with that role, plus the currently
    /// assigned member (even if they've since been deactivated) so editing doesn't drop them.
    /// </summary>
    public static async Task<List<SelectListItem>> MemberOptionsAsync(this AppDbContext db, MemberRole role, int? currentId)
    {
        var members = await db.Members
            .Where(m => m.Role == role && (m.IsActive || m.Id == currentId))
            .OrderBy(m => m.FullName)
            .AsNoTracking()
            .ToListAsync();

        return members
            .Select(m => new SelectListItem(
                m.IsActive ? $"{m.FullName} ({m.WorkEmail})" : $"{m.FullName} ({m.WorkEmail}) – inactive",
                m.Id.ToString(),
                m.Id == currentId))
            .ToList();
    }

    /// <summary>
    /// Server-side check that each selected person exists and has the right role, and that newly
    /// chosen people are active. (The dropdowns already limit choices; this stops tampered forms.)
    /// </summary>
    public static async Task ValidateAssignmentsAsync(this AppDbContext db, AppSystem system, ModelStateDictionary modelState, AppSystem? existing = null)
    {
        var checks = new (int? Id, int? PreviousId, MemberRole Role, string Field)[]
        {
            (system.BusinessOwnerId, existing?.BusinessOwnerId, MemberRole.BusinessOwner, "AppSystem.BusinessOwnerId"),
            (system.SystemAdminId, existing?.SystemAdminId, MemberRole.SystemAdmin, "AppSystem.SystemAdminId"),
            (system.ItDeveloperId, existing?.ItDeveloperId, MemberRole.ItDeveloper, "AppSystem.ItDeveloperId"),
        };

        foreach (var (id, previousId, role, field) in checks)
        {
            if (id is null)
                continue;

            var member = await db.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (member is null || member.Role != role)
                modelState.AddModelError(field, $"Choose a {role.DisplayName()} from the list.");
            else if (!member.IsActive && id != previousId)
                modelState.AddModelError(field, $"{member.FullName} is inactive and can't be assigned to systems.");
        }
    }
}
