using AppointmentsService.Data.Models;
using MediatR;

namespace AppointmentsService.Commands
{
    public class UpdateAppointmentCommand : IRequest<DbAppointment>
    {
        public DbAppointment Appointment { get; set; }
    }
}
