using Microsoft.EntityFrameworkCore;
using ProfilesService.Models;

namespace ProfilesService.Data
{
    public class ProfilesDbContext : DbContext
    {
        public virtual DbSet<DbDoctorModel> Doctors { get; set; }
        public virtual DbSet<DbPatientModel> Patients { get; set; }

        public ProfilesDbContext(DbContextOptions<ProfilesDbContext> options)
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
