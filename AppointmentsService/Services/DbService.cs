using AppointmentsService.Data;
using AppointmentsService.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace AppointmentsService.Services
{
    public class DbService
    {
        private readonly AppointmentsDbContext _dbContext;
        public DbService(AppointmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<DbAppointment>> GetAppointments(int pageNumber, int pageSize,
            DateTime? date, Guid? doctorId, Guid? serviceId, bool? Approved, Guid? officeId, Guid? patientId)
        {
            IQueryable<DbAppointment> query = _dbContext.Appointments;

            if (date != null)
            {
                query = query.Where(d => d.Date == date);
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

        public async Task<DbAppointment> UpdateAppointment(DbAppointment doctor)
        {
            var toEdit = await _dbContext.Appointments.FirstOrDefaultAsync(o => o.Id == doctor.Id);

            if (toEdit == null)
            {
                //throw new ProfilesException("Doctor not found", 404);
            }

            var oldApproval = toEdit.IsApproved;

            // todo: datetime slots check

            _dbContext.Entry(toEdit).CurrentValues.SetValues(doctor);

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
    }
}
