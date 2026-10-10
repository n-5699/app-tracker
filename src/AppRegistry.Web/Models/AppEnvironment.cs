using System.ComponentModel.DataAnnotations;

namespace AppRegistry.Web.Models;

/// <summary>One deployment of a system (a system can have several of the same type).</summary>
public class AppEnvironment
{
    public int Id { get; set; }

    /// <summary>Foreign key to the system this environment belongs to.</summary>
    public int AppSystemId { get; set; }
    public AppSystem? AppSystem { get; set; }

    [Required]
    public EnvironmentType Type { get; set; }

    /// <summary>Tells same-type environments apart, e.g. "Prod – East", "UAT 2".</summary>
    [StringLength(100)]
    public string? Label { get; set; }

    /// <summary>The link users click to open the application.</summary>
    [Required, StringLength(500), Url, Display(Name = "URL")]
    public string Url { get; set; } = string.Empty;

    /// <summary>Where it actually runs: Azure region, resource group, server name, etc.</summary>
    [Required, StringLength(500), Display(Name = "Deployment Location")]
    public string DeploymentLocation { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Notes { get; set; }
}
