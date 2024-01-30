using AppointmentsService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsService.Data
{
    public class AppointmentsDbContext : DbContext
    {
        public virtual DbSet<DbAppointment> Appointments { get; set; }

        public AppointmentsDbContext(DbContextOptions<AppointmentsDbContext> options)
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
