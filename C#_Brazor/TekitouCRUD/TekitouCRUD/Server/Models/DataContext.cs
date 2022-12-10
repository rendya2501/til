using Microsoft.EntityFrameworkCore;
using TekitouCRUD.Shared.Entities;

namespace TekitouCRUD.Server.Models;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public virtual DbSet<User> Users { get; set; } = null!;
}
