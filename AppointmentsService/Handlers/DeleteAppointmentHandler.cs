using AppointmentsService.Commands;
using AppointmentsService.Interfaces;
using MediatR;

namespace AppointmentsService.Handlers
{
    public class DeleteAppointmentHandler : IRequestHandler<DeleteAppointmentCommand>
    {
        private readonly IDbService _dbService;
        public DeleteAppointmentHandler(IDbService dbService)
        {
            _dbService = dbService;
        }
        public async Task Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
        {
            await _dbService.DeleteAppointment(request.Id);
        }
    }
}
