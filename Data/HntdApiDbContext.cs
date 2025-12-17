using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HntdApi.Models;

namespace HntdApi.Data;

public class HntdApiDbContext : IdentityDbContext<IdentityUser>
{
    public HntdApiDbContext(DbContextOptions<HntdApiDbContext> options) : base(options)
    {        
    }

    public DbSet<User> UserProfiles { get; set; }
    public DbSet<HauntedLocation> HauntedLocations { get; set; }
    public DbSet<LocationType> LocationTypes { get; set; }
    public DbSet<ActivityLevel> ActivityLevels { get; set; }
    public DbSet<ParanormalActivity> ParanormalActivities { get; set; }
    public DbSet<LocationActivity> LocationActivities { get; set; }
    public DbSet<UserFavorite> UserFavorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.IdentityUserId)
            .IsUnique();
        
        modelBuilder.Entity<LocationType>()
            .HasIndex(lt => lt.Name)
            .IsUnique();

        modelBuilder.Entity<HauntedLocation>()
            .HasIndex(hl => hl.Name)
            .IsUnique();

        modelBuilder.Entity<ActivityLevel>()
            .HasIndex(al => al.Name)
            .IsUnique();

        modelBuilder.Entity<ParanormalActivity>()
            .HasIndex(pa => pa.Name)
            .IsUnique();

        // Seed LocationTypes
        modelBuilder.Entity<LocationType>().HasData(
            new LocationType { Id = 1, Name = "Cemetery" },
            new LocationType { Id = 2, Name = "Hospital" },
            new LocationType { Id = 3, Name = "Asylum" },
            new LocationType { Id = 4, Name = "Prison" },
            new LocationType { Id = 5, Name = "House" },
            new LocationType { Id = 6, Name = "Hotel" },
            new LocationType { Id = 7, Name = "Battlefield" },
            new LocationType { Id = 8, Name = "Church" },
            new LocationType { Id = 9, Name = "Cave" },
            new LocationType { Id = 10, Name = "Theater" },
            new LocationType { Id = 11, Name = "School" }
        );

        // Seed ActivityLevels
        modelBuilder.Entity<ActivityLevel>().HasData(
            new ActivityLevel { Id = 1, Name = "Mild", Description = "Occasional unexplained occurrences" },
            new ActivityLevel { Id = 2, Name = "Moderate", Description = "Regular paranormal activity reported" },
            new ActivityLevel { Id = 3, Name = "High", Description = "Frequent and intense activity" }
        );

        // Seed ParanormalActivities
        modelBuilder.Entity<ParanormalActivity>().HasData(
            new ParanormalActivity { Id = 1, Name = "Apparitions", Description = "Visual ghost sightings", IconName = "ghost" },
            new ParanormalActivity { Id = 2, Name = "EVP", Description = "Electronic voice phenomena", IconName = "mic" },
            new ParanormalActivity { Id = 3, Name = "Cold Spots", Description = "Unexplained temperature drops", IconName = "thermometer" },
            new ParanormalActivity { Id = 4, Name = "Object Movement", Description = "Items moving on their own", IconName = "move" },
            new ParanormalActivity { Id = 5, Name = "Shadow Figures", Description = "Dark humanoid shapes", IconName = "shadow" },
            new ParanormalActivity { Id = 6, Name = "Disembodied Voices", Description = "Voices heard without source", IconName = "volume" }
        );
    }
}