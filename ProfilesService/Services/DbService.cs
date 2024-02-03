using CommonData.enums;
using CommonData.Messages;
using InnoClinicCommonData.Constants;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProfilesService.Data;
using ProfilesService.Data.Models;
using ProfilesService.Exceptions;
using Serilog;

namespace ProfilesService.Services
{
    public class DbService
    {
        private readonly ProfilesDbContext _dbContext;
        readonly IRequestClient<OfficeRequest> _officeRequestClient;
        readonly IRequestClient<SpecializationRequest> _specRequestClient;
        public DbService(ProfilesDbContext dbContext, IRequestClient<OfficeRequest> officeRequestClient, IRequestClient<SpecializationRequest> specRequestClient)
        {
            _dbContext = dbContext;
            _officeRequestClient = officeRequestClient;
            _specRequestClient = specRequestClient;
        }

        public async Task<IEnumerable<DbDoctorModel>> GetDoctors(int pageNumber, int pageSize,
            IEnumerable<Guid>? specializations, IEnumerable<Guid>? offices, IEnumerable<DoctorStatusEnum>? status, string? doctorName)
        {
            IQueryable<DbDoctorModel> query = _dbContext.Doctors;

            if (specializations != null && specializations.Any())
            {
                query = query.Where(d => specializations.Contains(d.SpecializationId));
            }
            if (offices != null && offices.Any())
            {
                query = query.Where(d => offices.Contains(d.OfficeId));
            }
            if (status != null && status.Any())
            {
                query = query.Where(d => status.Contains(d.Status));
            }
            if (!doctorName.IsNullOrEmpty())
            {
                query = query.Where(d => d.FirstName + ' ' + d.MiddleName + ' ' + d.LastName == doctorName); // todo: this is bs check
            }

            if (pageNumber != 0)
            {
                query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            }

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

            await SyncDoctorRedundancy(doctor);

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

            // todo: check if this works without double savechanges
            await _dbContext.SaveChangesAsync();

            await SyncDoctorRedundancy(doctor);

            _dbContext.Entry(doctor).State = EntityState.Modified;

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

            if (pageNumber != 0)
            {
                query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            }

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

            if (pageNumber != 0)
            {
                query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            }

            IEnumerable<DbReceptionistModel> patients = await query.AsNoTracking().ToListAsync();

            return patients;
        }

        public async Task<DbReceptionistModel> GetReceptionist(Guid id)
        {
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

            await SyncReceptionistRedundancy(receptionist);

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

            await SyncReceptionistRedundancy(receptionist);

            await _dbContext.SaveChangesAsync();

            return toEdit;
        }

        private async Task SyncDoctorRedundancy(DbDoctorModel doctor)
        {
            var officeUpdateTask = Task.Run(async () =>
            {
                try
                {
                    var result = await _officeRequestClient.GetResponse<OfficeUpdate>(new OfficeRequest()
                    {
                        Id = doctor.OfficeId
                    });

                    doctor.OfficeAddress = result.Message.Address;

                    return;
                }
                catch (RequestTimeoutException)
                {
                    Log.Error("Request timeout updating doctor's office address");
                }
                catch (RequestFaultException requestFaultException)
                {
                    Log.Error("Offices service encountered a problem: {@desc}", requestFaultException.Message);
                }
                doctor.OfficeAddress = StringConstants.OfficeUnreacheable;
            });

            var specializationUpdateTask = Task.Run(async () =>
            {
                try
                {
                    var result = await _specRequestClient.GetResponse<SpecializationUpdate>(new SpecializationRequest()
                    {
                        Id = doctor.SpecializationId
                    });

                    doctor.SpecializationName = result.Message.Name;

                    return;
                }
                catch (RequestTimeoutException)
                {
                    Log.Error("Request timeout updating doctor's specialization name");
                }
                catch (RequestFaultException requestFaultException)
                {
                    Log.Error("Services service encountered a problem: {@desc}", requestFaultException.Message);
                }
                doctor.SpecializationName = StringConstants.SpecializationUnreacheable;
            });

            try
            {
                await Task.WhenAll(officeUpdateTask, specializationUpdateTask);

                Log.Debug("Doctor's data synchronized");
            }
            catch (AggregateException ex)
            {
                Log.Error("Unhandled exception(s) while processing requests for other microservices: ");

                foreach (var e in ex.InnerExceptions)
                {
                    Log.Error(e.Message);
                }
            }
        }

        private async Task SyncReceptionistRedundancy(DbReceptionistModel receptionist)
        {
            var officeUpdateTask = Task.Run(async () =>
            {
                try
                {
                    var result = await _officeRequestClient.GetResponse<OfficeUpdate>(new OfficeRequest()
                    {
                        Id = receptionist.OfficeId
                    });

                    receptionist.OfficeAddress = result.Message.Address;

                    return;
                }
                catch (RequestTimeoutException)
                {
                    Log.Error("Request timeout updating doctor's office address");
                }
                catch (RequestFaultException requestFaultException)
                {
                    Log.Error("Offices service encountered a problem: {@desc}", requestFaultException.Message);
                }

                receptionist.OfficeAddress = StringConstants.OfficeUnreacheable;
            });

            try
            {
                await officeUpdateTask;

                Log.Debug("Receptionist's data synchronized");
            }
            catch (Exception ex)
            {
                Log.Error("Unhandled exception(s) while processing requests for other microservices: {@desc}", ex.Message);
            }
        }
    }
}
