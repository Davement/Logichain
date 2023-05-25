using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models;

namespace Migrations;

public class DbContext : IdentityDbContext
{
    private readonly string _connectionString;
    private readonly IConfiguration _configuration;

    public DbSet<Location> Locations { get; set; }

    public DbContext(DbContextOptions options, IConfiguration configuration, string connectionString = "LogichainDb") : base(options)
    {
        _configuration = configuration;
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString(_connectionString));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ConfigureCustomEntities(builder);
        AddDefaultValueToBaseEntity(builder);
        base.OnModelCreating(builder);
    }

    private static void ConfigureCustomEntities(ModelBuilder builder)
    {
        builder.Entity<Location>()
            .HasOne(e => e.Parent)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void AddDefaultValueToBaseEntity(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes().Where(x => x.ClrType.IsSubclassOf(typeof(BaseEntity))))
        {
            builder.Entity(entityType.Name, x =>
            {
                x.Property("Created")
                    .HasDefaultValueSql("getutcdate()");

                x.Property("Updated")
                    .HasDefaultValueSql("getutcdate()");
            });
        }
    }
}