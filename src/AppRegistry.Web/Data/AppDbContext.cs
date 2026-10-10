using AppRegistry.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AppRegistry.Web.Data;

/// <summary>
/// The bridge between C# and the database. Each DbSet becomes a table.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppSystem> Systems => Set<AppSystem>();
    public DbSet<AppEnvironment> Environments => Set<AppEnvironment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppSystem>(e =>
        {
            e.ToTable("Systems");
            e.HasIndex(s => s.Name).IsUnique();   // no duplicate system names

            // One system has many environments; deleting a system deletes its environments.
            e.HasMany(s => s.Environments)
             .WithOne(env => env.AppSystem)
             .HasForeignKey(env => env.AppSystemId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AppEnvironment>(e =>
        {
            e.ToTable("Environments");
            // Store the type as readable text ("Production") instead of a number (4).
            e.Property(env => env.Type).HasConversion<string>().HasMaxLength(20);
        });
    }

    /// <summary>Sets CreatedAt / UpdatedAt automatically whenever a system (or one of its environments) is saved.</summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // Adding, editing or deleting an environment counts as an update to its system.
        var touchedSystemIds = ChangeTracker.Entries<AppEnvironment>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(e => e.Entity.AppSystemId)
            .Distinct()
            .ToList();
        foreach (var systemId in touchedSystemIds)
        {
            var system = await Systems.FindAsync([systemId], cancellationToken);
            if (system is not null && Entry(system).State == EntityState.Unchanged)
                system.UpdatedAt = now;
        }

        foreach (var entry in ChangeTracker.Entries<AppSystem>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
