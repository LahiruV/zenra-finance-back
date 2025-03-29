using Microsoft.EntityFrameworkCore;
using zenra_finance_back.Models;

namespace zenra_finance_back.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<YourEntity> YourEntities { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Finance> Finances { get; set; }
    }
}
