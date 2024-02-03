using AppointmentsService.Data;
using AppointmentsService.Data.Models;
using CommonData.Messages;
using InnoClinicCommonData.Constants;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Numerics;
using static MassTransit.ValidationResultExtensions;

namespace AppointmentsService.Services
{
    public class DbService
    {
        private readonly IRequestClient<OfficeRequest> _officeRequestClient;
        private readonly IRequestClient<DoctorRequest> _doctorRequestClient;
        private readonly IRequestClient<ServiceRequest> _serviceRequestClient;
        private readonly IRequestClient<PatientRequest> _patientRequestClient;
        private readonly AppointmentsDbContext _dbContext;

        public DbService(AppointmentsDbContext dbContext,
            IRequestClient<OfficeRequest> officeRequestClient, IRequestClient<DoctorRequest> doctorRequestClient,
            IRequestClient<ServiceRequest> serviceRequestClient, IRequestClient<PatientRequest> patientRequestClient)
        {
            _dbContext = dbContext;
            _officeRequestClient = officeRequestClient;
            _doctorRequestClient = doctorRequestClient;
            _serviceRequestClient = serviceRequestClient;
            _patientRequestClient = patientRequestClient;
        }

        public async Task<IEnumerable<DbAppointment>> GetAppointments(int pageNumber, int pageSize,
            DateTime? date, Guid? doctorId, Guid? serviceId, bool? Approved, Guid? officeId, Guid? patientId)
        {
            IQueryable<DbAppointment> query = _dbContext.Appointments;

            if (date != null)
            {
                query = query.Where(d => d.Date.Date == date.Value.Date);
            }
            if (officeId != null)
            {
                query = query.Where(d => d.OfficeId == officeId);
            }
            if (serviceId != null)
            {
                query = query.Where(d => d.ServiceId == serviceId);
            }
            if (Approved != null)
            {
                query = query.Where(d => d.IsApproved == Approved);
            }
            if (doctorId != null)
            {
                query = query.Where(d => d.DoctorId == doctorId);
            }
            if (patientId != null)
            {
                query = query.Where(d => d.PatientId == patientId);
            }

            if (pageNumber != 0)
            {
                query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            }

            IEnumerable<DbAppointment> appointments = await query.AsNoTracking().ToListAsync();

            return appointments;
        }

        public async Task<DbAppointment> GetAppointment(Guid id)
        {
            var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(d => d.Id == id);

            if (appointment == null)
            {
                //throw new ProfilesException("Doctor not found", 404);
            }

            return appointment;
        }

        public async Task<DbAppointment> AddAppointment(DbAppointment appointment)
        {
            appointment.Id = Guid.NewGuid();

            await SyncAppointmentRedundancy(appointment);

            _dbContext.Appointments.Add(appointment);

            // todo: datetime slots check

            await _dbContext.SaveChangesAsync();

            return appointment;
        }

        public async Task<DbAppointment> DeleteAppointment(Guid id)
        {
            var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(o => o.Id == id);

            if (appointment == null)
            {
                //throw new ProfilesException("Doctor not found", 404);
            }

            _dbContext.Appointments.Remove(appointment);

            await _dbContext.SaveChangesAsync();

            return appointment;
        }

        public async Task<DbAppointment> UpdateAppointment(DbAppointment appointment)
        {
            var toEdit = await _dbContext.Appointments.FirstOrDefaultAsync(o => o.Id == appointment.Id);

            if (toEdit == null)
            {
                //throw new ProfilesException("Doctor not found", 404);
            }

            var oldApproval = toEdit.IsApproved;

            // todo: datetime slots check

            _dbContext.Entry(toEdit).CurrentValues.SetValues(appointment);

            // We have separate method for approving (needed for auth)
            toEdit.IsApproved = oldApproval;

            await _dbContext.SaveChangesAsync();

            return toEdit;
        }

        public async Task ChangeAppointmentApproval(Guid id, bool isApproved)
        {
            var toEdit = await _dbContext.Appointments.FirstOrDefaultAsync(o => o.Id == id);

            if (toEdit == null)
            {
                //throw new ProfilesException("Doctor not found", 404);
            }

            toEdit.IsApproved = isApproved;

            await _dbContext.SaveChangesAsync();
        }

        private async Task SyncAppointmentRedundancy(DbAppointment appointment)
        {
            var officeUpdateTask = new Task(async () =>
            {
                try
                {
                    var result = await _officeRequestClient.GetResponse<OfficeUpdate>(new OfficeRequest()
                    {
                        Id = appointment.OfficeId
                    });

                    appointment.OfficeAddress = result.Message.Address;

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

                appointment.OfficeAddress = StringConstants.OfficeUnreacheable;
            });

            var doctorUpdateTask = new Task(async () =>
            {
                try
                {
                    var result = await _doctorRequestClient.GetResponse<DoctorUpdate>(new DoctorRequest()
                    {
                        Id = appointment.DoctorId
                    });

                    appointment.DoctorFirstName = result.Message.FirstName;
                    appointment.DoctorMiddleName = result.Message.MiddleName;
                    appointment.DoctorLastName = result.Message.LastName;

                    return;
                }
                catch (RequestTimeoutException)
                {
                    Log.Error("Request timeout updating doctor's data");
                }
                catch (RequestFaultException requestFaultException)
                {
                    Log.Error("Profiles service encountered a problem: {@desc}", requestFaultException.Message);
                }

                appointment.DoctorFirstName = StringConstants.ProfileUnreacheable;
                appointment.DoctorMiddleName = StringConstants.ProfileUnreacheable;
                appointment.DoctorLastName = StringConstants.ProfileUnreacheable;
            });

            var patientUpdateTask = new Task(async () =>
            {
                try
                {
                    var result = await _patientRequestClient.GetResponse<PatientUpdate>(new PatientRequest()
                    {
                        Id = appointment.PatientId
                    });

                    appointment.PatientFirstName = result.Message.FirstName;
                    appointment.PatientMiddleName = result.Message.MiddleName;
                    appointment.PatientLastName = result.Message.LastName;

                    return;
                }
                catch (RequestTimeoutException)
                {
                    Log.Error("Request timeout updating patient's data");
                }
                catch (RequestFaultException requestFaultException)
                {
                    Log.Error("Profiles service encountered a problem: {@desc}", requestFaultException.Message);
                }

                appointment.PatientFirstName = StringConstants.ProfileUnreacheable;
                appointment.PatientMiddleName = StringConstants.ProfileUnreacheable;
                appointment.PatientLastName = StringConstants.ProfileUnreacheable;
            });

            var serviceUpdateTask = new Task(async () =>
            {
                try
                {
                    var result = await _serviceRequestClient.GetResponse<ServiceUpdate>(new ServiceRequest()
                    {
                        Id = appointment.ServiceId
                    });

                    appointment.ServiceName = result.Message.Name;

                    return;
                }
                catch (RequestTimeoutException)
                {
                    Log.Error("Request timeout updating service data");
                }
                catch (RequestFaultException requestFaultException)
                {
                    Log.Error("Services service encountered a problem: {@desc}", requestFaultException.Message);
                }

                appointment.ServiceName = StringConstants.ServiceUnreacheable;
            });

            try
            {
                await Task.WhenAll(serviceUpdateTask, doctorUpdateTask, patientUpdateTask, patientUpdateTask);

                Log.Debug("Appointment data synchronized");
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
    }
}
