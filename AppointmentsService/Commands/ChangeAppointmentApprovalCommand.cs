using MediatR;

namespace AppointmentsService.Commands
{
    public class ChangeAppointmentApprovalCommand : IRequest
    {
        public Guid Id { get; set; }
        public bool IsApproved { get; set; }
    }
}
