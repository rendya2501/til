using Microsoft.EntityFrameworkCore;
using WebAPISample.Entities;

namespace WebAPISample.Context;

public class DatContext : DbContext
{
    public DatContext(DbContextOptions<DatContext> options) : base(options) { }
    public virtual DbSet<Category> Category { get; set; } = null!;
    public virtual DbSet<Product> Product { get; set; } = null!;
}