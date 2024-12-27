using EcoState.Domain;
using Microsoft.EntityFrameworkCore;

namespace EcoState.Interfaces;

public interface IDbContext
{
    public DbSet<Concentration> Concentrations { get; set; } 
    public DbSet<Emission> Emissions { get; set; } 
    public DbSet<User> Users { get; set; } 
    public DbSet<Weather> Weathers { get; set; }
}