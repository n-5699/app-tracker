namespace AppRegistry.Web.Models;

/// <summary>Records that get CreatedAt / UpdatedAt filled in automatically by AppDbContext.</summary>
public interface ITimestamped
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}
