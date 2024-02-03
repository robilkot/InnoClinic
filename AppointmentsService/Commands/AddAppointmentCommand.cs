using AppointmentsService.Data.Models;
using MediatR;

namespace AppointmentsService.Commands
{
    public class AddAppointmentCommand : IRequest<DbAppointment>
    {
        public required DbAppointment Appointment { get; set; }
    }
}
