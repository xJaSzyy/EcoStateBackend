using EcoState.Domain;
using EcoState.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcoState.Context;

public class ApplicationDbContext : DbContext, IDbContext
{
    public DbSet<Concentration> Concentrations { get; set; } = null!;
    public DbSet<ConcentrationList> ConcentrationLists { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Weather> Weathers { get; set; } = null!;
    public DbSet<Role> Roles { get; set; }
    
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Role adminRole = new Role { Id = 1, Name = "admin" };
        Role userRole = new Role { Id = 2, Name = "user" };
        modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
        
        base.OnModelCreating(modelBuilder);
    }
}