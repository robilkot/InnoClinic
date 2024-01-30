using AppointmentsService.Commands;
using AppointmentsService.Services;
using MediatR;

namespace AppointmentsService.Handlers
{
    public class ChangeAppointmentApprovalHandler : IRequestHandler<ChangeAppointmentApprovalCommand>
    {
        private readonly DbService _dbService;
        public ChangeAppointmentApprovalHandler(DbService dbService)
        {
            _dbService = dbService;
        }
        public async Task Handle(ChangeAppointmentApprovalCommand request, CancellationToken cancellationToken)
        {
            await _dbService.ChangeAppointmentApproval(request.Id, request.IsApproved);
        }
    }
}
