using System.ComponentModel.DataAnnotations;

namespace AppRegistry.Web.Models;

/// <summary>An application/system maintained by the agency.</summary>
public class AppSystem : ITimestamped
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    // People are picked from the Members list. Each is a link (foreign key) to a Member with the matching role.

    /// <summary>Product owner — the person who gives the requirements. Required.</summary>
    [Required(ErrorMessage = "Choose a Business Owner."), Display(Name = "Business Owner")]
    public int? BusinessOwnerId { get; set; }
    public Member? BusinessOwner { get; set; }

    [Display(Name = "System Admin")]
    public int? SystemAdminId { get; set; }
    public Member? SystemAdmin { get; set; }

    /// <summary>The IT developer who maintains the system.</summary>
    [Display(Name = "IT Developer")]
    public int? ItDeveloperId { get; set; }
    public Member? ItDeveloper { get; set; }

    [Display(Name = "Created")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Last Updated")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>Every place this system is deployed. One system → many environments.</summary>
    public List<AppEnvironment> Environments { get; set; } = new();
}
