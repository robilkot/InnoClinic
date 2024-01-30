using AppointmentsService.Data.Models;
using MediatR;

namespace AppointmentsService.Commands
{
    public class AddAppointmentCommand : IRequest<DbAppointment>
    {
        public DbAppointment Appointment { get; set; }
    }
}
