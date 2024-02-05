using AppointmentsService.Data.Models;

namespace AppointmentsService.Interfaces
{
    public interface IDbService
    {
        Task<IEnumerable<DbAppointment>> GetAppointments(int pageNumber, int pageSize,
            DateTime? date, Guid? doctorId, Guid? serviceId, bool? approved, Guid? officeId, Guid? patientId);

        Task<DbAppointment> GetAppointment(Guid id);

        Task<DbAppointment> AddAppointment(DbAppointment appointment);

        Task DeleteAppointment(Guid id);

        Task<DbAppointment> UpdateAppointment(DbAppointment appointment);

        Task ChangeAppointmentApproval(Guid id, bool isApproved);
    }
}
