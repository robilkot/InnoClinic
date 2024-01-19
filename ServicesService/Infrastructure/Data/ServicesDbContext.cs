using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ServicesService.Domain.Entities;

namespace ServicesService.Infrastructure.Data
{
    public class ServicesDbContext : DbContext
    {
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Specialization> Specializations { get; set; }

        public ServicesDbContext(DbContextOptions<ServicesDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
