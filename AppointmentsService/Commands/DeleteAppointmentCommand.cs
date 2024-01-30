using MediatR;

namespace AppointmentsService.Commands
{
    public class DeleteAppointmentCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
