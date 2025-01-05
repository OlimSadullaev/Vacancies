using Microsoft.EntityFrameworkCore;
using Vacancies.Models;

namespace Vacancies.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Grant> Grants { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure many-to-many relationship between Grant and Category
            builder.Entity<Grant>()
                .HasMany(g => g.Categories)
                .WithMany(c => c.Grants)
                .UsingEntity(j => j.ToTable("GrantCategories"));

            // Add indexes
            builder.Entity<Grant>()
                .HasIndex(g => g.Country);

            builder.Entity<Category>()
                .HasIndex(c => c.Name);
        }
    }
}
