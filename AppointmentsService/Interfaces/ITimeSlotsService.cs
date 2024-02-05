using AppointmentsService.Data.Models;
using AppointmentsService.Models;

namespace AppointmentsService.Interfaces
{
    public interface ITimeSlotsService
    {
        Task<bool> AppointmentTimeIsValid(DbAppointment appointment);
        Task<IEnumerable<ClientTimeSlot>> GetTimeSlots(DateTime date, Guid officeId, Guid doctorId);
    }
}
