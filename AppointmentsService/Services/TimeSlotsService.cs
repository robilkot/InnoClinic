using AppointmentsService.Data;
using AppointmentsService.Data.Models;
using AppointmentsService.Interfaces;
using AppointmentsService.Models;
using CommonData.Constants;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsService.Services
{
    public class TimeSlotsService : ITimeSlotsService
    {
        private readonly AppointmentsDbContext _dbContext;

        public TimeSlotsService(AppointmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AppointmentTimeIsValid(DbAppointment appointment)
        {
            var workingHours = TimeSlotsConstants.WorkingHours[(int)appointment.Date.DayOfWeek];

            if (workingHours == null)
            {
                return false;
            }

            if (appointment.Date.Minute % TimeSlotsConstants.TimeSlotLength != 0)
            {
                return false;
            }

            var slots = await GetTimeSlots(appointment.Date, appointment.OfficeId, appointment.DoctorId);

            var hasConflict = slots.Any(slot =>
                      slot.Occupied
                   && slot.Hour == appointment.Date.Hour
                   && slot.Minute >= appointment.Date.Minute
                   && slot.Minute < appointment.Date.Minute + TimeSlotsConstants.TimeSlotLength * appointment.TimeSlotSize
                   && slot.AppointmentId != appointment.Id);

            return hasConflict;
        }

        public async Task<IEnumerable<ClientTimeSlot>> GetTimeSlots(DateTime date, Guid officeId, Guid doctorId)
        {
            var workingHours = TimeSlotsConstants.WorkingHours[(int)date.DayOfWeek];

            if (workingHours == null)
            {
                return Enumerable.Empty<ClientTimeSlot>();
            }

            // todo: this is duplicate code just to resolve circular dependency on idbservice
            IQueryable<DbAppointment> query = _dbContext.Appointments
                .Where(d => d.Date.Date == date.Date)
                .Where(d => d.OfficeId == officeId)
                .Where(d => d.DoctorId == doctorId);

            IEnumerable<DbAppointment>? appointmentsToCheck = await query.AsNoTracking().ToListAsync();


            var timeSlots = new List<ClientTimeSlot>();

            for (int hour = workingHours.Value.Item1; hour < workingHours.Value.Item2; hour++)
            {
                for (int minute = 0; minute < 60; minute += TimeSlotsConstants.TimeSlotLength)
                {
                    ClientTimeSlot slot = new()
                    {
                        Hour = hour,
                        Minute = minute,
                        Occupied = false
                    };

                    timeSlots.Add(slot);
                }
            }

            foreach (var appointment in appointmentsToCheck)
            {
                var occupiedSlots = timeSlots.Where(t =>
                        t.Hour == appointment.Date.Hour
                    && t.Minute >= appointment.Date.Minute
                    && t.Minute < appointment.Date.Minute + TimeSlotsConstants.TimeSlotLength * appointment.TimeSlotSize);

                foreach (var slot in occupiedSlots)
                {
                    slot.Occupied = true;
                    slot.AppointmentId = appointment.Id;
                }
            }

            return timeSlots;
        }
    }
}
