using Microsoft.EntityFrameworkCore;
using zenra_finance_back.Models;

namespace zenra_finance_back.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Finance> Finances { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Expense>().ToTable("expenses");
            modelBuilder.Entity<Finance>().ToTable("finances");

            base.OnModelCreating(modelBuilder);
        }
    }
}
