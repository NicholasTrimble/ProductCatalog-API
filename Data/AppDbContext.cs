using TestProject.Models;
using Microsoft.EntityFrameworkCore;

namespace TestProject.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
}