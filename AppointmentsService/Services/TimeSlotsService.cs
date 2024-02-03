using AppointmentsService.Data.Models;
using InnoClinicCommonData.Constants;

namespace AppointmentsService.Services
{
    public class TimeSlotsService
    {
        private readonly DbService _dbService;

        public TimeSlotsService(DbService dbService)
        {
            _dbService = dbService;
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

        public async Task<IEnumerable<TimeSlot>> GetTimeSlots(DateTime date, Guid officeId, Guid doctorId)
        {
            var workingHours = TimeSlotsConstants.WorkingHours[(int)date.DayOfWeek];

            if (workingHours == null)
            {
                return Enumerable.Empty<TimeSlot>();
            }

            var appointmentsToCheck = await _dbService.GetAppointments(0, 0, date, doctorId, null, null, officeId, null);

            var timeSlots = new List<TimeSlot>();

            for (int hour = workingHours.Value.Item1; hour < workingHours.Value.Item2; hour++)
            {
                for (int minute = 0; minute < 60; minute += TimeSlotsConstants.TimeSlotLength)
                {
                    TimeSlot slot = new()
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
