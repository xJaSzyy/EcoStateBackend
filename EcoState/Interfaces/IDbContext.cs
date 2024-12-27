using EcoState.Domain;
using Microsoft.EntityFrameworkCore;

namespace EcoState.Interfaces;

public interface IDbContext
{
    public DbSet<Concentration> Concentrations { get; set; } 
    public DbSet<ConcentrationList> ConcentrationLists { get; set; } 
    public DbSet<User> Users { get; set; } 
    public DbSet<Weather> Weathers { get; set; }
}