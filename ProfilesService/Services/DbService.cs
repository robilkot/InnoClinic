using ProfilesService.Data;
using ProfilesService.Models;
using ProfilesService.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ProfilesService.Services
{
    public class DbService
    {
        private readonly ProfilesDbContext _dbContext;
        public DbService(ProfilesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<DbDoctorModel>> GetDoctors(int pageNumber, int pageSize, IEnumerable<Guid>? specializations, IEnumerable<Guid>? offices, IEnumerable<DoctorStatusEnum>? status, string? doctorName)
        {
            IQueryable<DbDoctorModel> query = _dbContext.Doctors;

            if(specializations != null)
            {
                query = query.Where(d => specializations.Contains(d.SpecializationId));
            }
            if(offices != null)
            {
                query = query.Where(d => offices.Contains(d.OfficeId));
            }
            if (status != null)
            {
                query = query.Where(d => status.Contains(d.Status));
            }
            if (doctorName != null)
            {
                query = query.Where(d => d.FirstName + ' ' + d.MiddleName + ' ' + d.LastName == doctorName); // todo: this is bs check
            }

            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            IEnumerable<DbDoctorModel> doctors = await query.AsNoTracking().ToListAsync();

            return doctors;
        }

        public async Task<DbDoctorModel> GetDoctor(Guid id)
        {
            // todo: is this obj being tracked?
            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
            {
                throw new ProfilesException("Doctor not found", 404);
            }

            return doctor;
        }

        public async Task<DbDoctorModel> AddDoctor(DbDoctorModel doctor)
        {
            doctor.Id = Guid.NewGuid();

            _dbContext.Doctors.Add(doctor);

            await _dbContext.SaveChangesAsync();

            return doctor;
        }

        public async Task<DbDoctorModel> DeleteDoctor(Guid id)
        {
            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(o => o.Id == id);

            if (doctor == null)
            {
                throw new ProfilesException("Doctor not found", 404);
            }

            _dbContext.Doctors.Remove(doctor);

            await _dbContext.SaveChangesAsync();

            return doctor;
        }

        public async Task<DbDoctorModel> UpdateDoctor(DbDoctorModel doctor)
        {
            var toEdit = await _dbContext.Doctors.FirstOrDefaultAsync(o => o.Id == doctor.Id);

            if (toEdit == null)
            {
                throw new ProfilesException("Doctor not found", 404);
            }

            _dbContext.Entry(toEdit).CurrentValues.SetValues(doctor);

            await _dbContext.SaveChangesAsync();

            return toEdit;
        }
    }
}
