using EcoState.Domain;
using Microsoft.EntityFrameworkCore;

namespace EcoState.Context;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<Concentration> Concentrations { get; set; } = null!;
    
    public virtual DbSet<ConcentrationList> ConcentrationLists { get; set; } = null!;
    
    public virtual DbSet<User> Users { get; set; } = null!;
    
    public virtual DbSet<Weather> Weathers { get; set; } = null!;
    
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }
}