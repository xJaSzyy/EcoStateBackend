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
    
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }
}