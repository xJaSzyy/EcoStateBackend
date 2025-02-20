using EcoState.Domain;
using EcoState.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcoState.Context;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<Emission> Emissions { get; set; } = null!;
    public virtual DbSet<Concentration> Concentrations { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Weather> Weathers { get; set; } = null!;
    
    public ApplicationDbContext() {  }
    
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }
}