using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleAppSample.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsoleAppSample.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(){}
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
    }
}