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

        //public async Task<IEnumerable<TimeSlot>> GetAvailableTimeSlots(DateTime date, int TimeSlotSize, Guid officeId, Guid doctorId)
        //{
        //    var workingHours = TimeSlotsConstants.WorkingHours[(int)date.DayOfWeek];

        //    if (workingHours == null)
        //    {
        //        return Enumerable.Empty<TimeSlot>();
        //    }

        //    var conflictingAppointments = _dbService.GetAppointments(0, 0, date, doctorId, null, null, officeId, null);

        //    var timeSlots = new List<TimeSlot>();

        //    // todo: send list of occupied timeslots (less space consumption than to send everything)
        //    //for (int hour = )

        //}
    }
}
