namespace AppRegistry.Web.Models;

public static class EnvironmentTypeExtensions
{
    /// <summary>Bootstrap badge color for each environment type, so Production stands out.</summary>
    public static string BadgeClass(this EnvironmentType type) => type switch
    {
        EnvironmentType.Production => "text-bg-success",
        EnvironmentType.Staging => "text-bg-warning",
        EnvironmentType.UAT => "text-bg-info",
        EnvironmentType.Test => "text-bg-primary",
        _ => "text-bg-secondary"
    };
}
