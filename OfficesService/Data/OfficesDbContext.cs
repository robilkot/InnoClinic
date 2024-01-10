using Microsoft.EntityFrameworkCore;
using OfficesService.Models;

namespace OfficesService.Data
{
    public class OfficesDbContext : DbContext
    {
        public virtual DbSet<DbOfficeModel> Offices { get; set; }
        public virtual DbSet<DbImageModel> Photos { get; set; }

        public OfficesDbContext(DbContextOptions<OfficesDbContext> options)
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
