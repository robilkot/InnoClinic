using AppointmentsService.Data.Models;
using MediatR;

namespace AppointmentsService.Queries
{
    public class GetAppointmentByIdQuery : IRequest<DbAppointment>
    {
        public Guid Id { get; set; }
    }
}
