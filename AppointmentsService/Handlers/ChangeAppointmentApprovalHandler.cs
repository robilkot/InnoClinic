using AppointmentsService.Commands;
using AppointmentsService.Interfaces;
using MediatR;

namespace AppointmentsService.Handlers
{
    public class ChangeAppointmentApprovalHandler : IRequestHandler<ChangeAppointmentApprovalCommand>
    {
        private readonly IDbService _dbService;
        public ChangeAppointmentApprovalHandler(IDbService dbService)
        {
            _dbService = dbService;
        }
        public async Task Handle(ChangeAppointmentApprovalCommand request, CancellationToken cancellationToken)
        {
            await _dbService.ChangeAppointmentApproval(request.Id, request.IsApproved);
        }
    }
}
