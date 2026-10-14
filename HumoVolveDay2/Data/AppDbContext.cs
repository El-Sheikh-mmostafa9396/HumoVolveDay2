using HumoVolveDay2.Models;
using Microsoft.EntityFrameworkCore;

namespace HumoVolveDay2.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        public DbSet<Product> Product => Set<Product>();

        public DbSet<Category> Category => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<Product>()
                .HasQueryFilter(p => !p.IsDeleted);

            
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name);
        }

    }
}
