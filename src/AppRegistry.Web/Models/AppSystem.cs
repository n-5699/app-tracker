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

    /// <summary>Product owner — the person who gives the requirements.</summary>
    [Required, StringLength(200), Display(Name = "Business Owner")]
    public string BusinessOwner { get; set; } = string.Empty;

    [StringLength(200), Display(Name = "System Admin")]
    public string? SystemAdmin { get; set; }

    /// <summary>The IT developer who maintains the system.</summary>
    [StringLength(200), Display(Name = "IT Developer")]
    public string? ItDeveloper { get; set; }

    [Display(Name = "Created")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Last Updated")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>Every place this system is deployed. One system → many environments.</summary>
    public List<AppEnvironment> Environments { get; set; } = new();
}
