using Microsoft.EntityFrameworkCore;
using ServicesService.Domain.Entities;

namespace ServicesService.Infrastructure.Data
{
    public class ServicesDbContext : DbContext
    {
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Specialization> Specializations { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        public ServicesDbContext(DbContextOptions<ServicesDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Service>().Property(o => o.Price).HasPrecision(12, 10);

            base.OnModelCreating(builder);
        }
    }
}
