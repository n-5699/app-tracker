using System.ComponentModel.DataAnnotations;

namespace AppRegistry.Web.Models;

/// <summary>A person who can be assigned to systems as Business Owner, System Admin or IT Developer.</summary>
public class Member : ITimestamped
{
    public int Id { get; set; }

    [Required]
    public MemberRole Role { get; set; }

    [Required, StringLength(200), Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(256), EmailAddress, Display(Name = "Work Email")]
    public string WorkEmail { get; set; } = string.Empty;

    [Required, StringLength(50), Phone, Display(Name = "Work Phone")]
    public string WorkPhone { get; set; } = string.Empty;

    /// <summary>Inactive members keep their existing system assignments but can't be picked for new ones.</summary>
    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Created")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Last Updated")]
    public DateTime UpdatedAt { get; set; }
}
