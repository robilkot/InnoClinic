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


        public async Task<IEnumerable<DbPatientModel>> GetPatients(int pageNumber, int pageSize, string? name)
        {
            IQueryable<DbPatientModel> query = _dbContext.Patients;

            if (name != null)
            {
                query = query.Where(d => d.FirstName + ' ' + d.MiddleName + ' ' + d.LastName == name); // todo: this is bs check
            }

            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            IEnumerable<DbPatientModel> patients = await query.AsNoTracking().ToListAsync();

            return patients;
        }

        public async Task<DbPatientModel> GetPatient(Guid id)
        {
            // todo: is this obj being tracked?
            var doctor = await _dbContext.Patients.FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
            {
                throw new ProfilesException("Doctor not found", 404);
            }

            return doctor;
        }

        public async Task<DbPatientModel> AddPatient(DbPatientModel patient)
        {
            patient.Id = Guid.NewGuid();

            _dbContext.Patients.Add(patient);

            await _dbContext.SaveChangesAsync();

            return patient;
        }

        public async Task<DbPatientModel> DeletePatient(Guid id)
        {
            var patient = await _dbContext.Patients.FirstOrDefaultAsync(o => o.Id == id);

            if (patient == null)
            {
                throw new ProfilesException("Patient not found", 404);
            }

            _dbContext.Patients.Remove(patient);

            await _dbContext.SaveChangesAsync();

            return patient;
        }

        public async Task<DbPatientModel> UpdatePatient(DbPatientModel patient)
        {
            var toEdit = await _dbContext.Patients.FirstOrDefaultAsync(o => o.Id == patient.Id);

            if (toEdit == null)
            {
                throw new ProfilesException("Patient not found", 404);
            }

            _dbContext.Entry(toEdit).CurrentValues.SetValues(patient);

            await _dbContext.SaveChangesAsync();

            return toEdit;
        }


        public async Task<IEnumerable<DbReceptionistModel>> GetReceptionists(int pageNumber, int pageSize, string? name)
        {
            IQueryable<DbReceptionistModel> query = _dbContext.Receptionists;

            if (name != null)
            {
                query = query.Where(d => d.FirstName + ' ' + d.MiddleName + ' ' + d.LastName == name); // todo: this is bs check
            }

            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            IEnumerable<DbReceptionistModel> patients = await query.AsNoTracking().ToListAsync();

            return patients;
        }

        public async Task<DbReceptionistModel> GetReceptionist(Guid id)
        {
            // todo: is this obj being tracked?
            var receptionist = await _dbContext.Receptionists.FirstOrDefaultAsync(d => d.Id == id);

            if (receptionist == null)
            {
                throw new ProfilesException("Receptionist not found", 404);
            }

            return receptionist;
        }

        public async Task<DbReceptionistModel> AddReceptionist(DbReceptionistModel receptionist)
        {
            receptionist.Id = Guid.NewGuid();

            _dbContext.Receptionists.Add(receptionist);

            await _dbContext.SaveChangesAsync();

            return receptionist;
        }

        public async Task<DbReceptionistModel> DeleteReceptionist(Guid id)
        {
            var receptionist = await _dbContext.Receptionists.FirstOrDefaultAsync(o => o.Id == id);

            if (receptionist == null)
            {
                throw new ProfilesException("Receptionist not found", 404);
            }

            _dbContext.Receptionists.Remove(receptionist);

            await _dbContext.SaveChangesAsync();

            return receptionist;
        }

        public async Task<DbReceptionistModel> UpdateReceptionist(DbReceptionistModel receptionist)
        {
            var toEdit = await _dbContext.Receptionists.FirstOrDefaultAsync(o => o.Id == receptionist.Id);

            if (toEdit == null)
            {
                throw new ProfilesException("Receptionist not found", 404);
            }

            _dbContext.Entry(toEdit).CurrentValues.SetValues(receptionist);

            await _dbContext.SaveChangesAsync();

            return toEdit;
        }
    }
}
