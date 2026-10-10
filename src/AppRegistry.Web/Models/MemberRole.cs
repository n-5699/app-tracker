using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AppRegistry.Web.Models;

/// <summary>The role a member plays on systems.</summary>
public enum MemberRole
{
    [Display(Name = "Business Owner")]
    BusinessOwner,

    [Display(Name = "System Admin")]
    SystemAdmin,

    [Display(Name = "IT Developer")]
    ItDeveloper
}

public static class MemberRoleExtensions
{
    /// <summary>Readable name, e.g. "Business Owner" instead of "BusinessOwner".</summary>
    public static string DisplayName(this MemberRole role) =>
        typeof(MemberRole).GetField(role.ToString())?.GetCustomAttribute<DisplayAttribute>()?.Name ?? role.ToString();

    /// <summary>Bootstrap badge color for each role.</summary>
    public static string BadgeClass(this MemberRole role) => role switch
    {
        MemberRole.BusinessOwner => "text-bg-primary",
        MemberRole.SystemAdmin => "text-bg-warning",
        _ => "text-bg-info"
    };
}
