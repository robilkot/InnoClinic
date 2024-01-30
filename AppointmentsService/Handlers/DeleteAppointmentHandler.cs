using AppointmentsService.Commands;
using AppointmentsService.Services;
using MediatR;

namespace AppointmentsService.Handlers
{
    public class DeleteAppointmentHandler : IRequestHandler<DeleteAppointmentCommand>
    {
        private readonly DbService _dbService;
        public DeleteAppointmentHandler(DbService dbService)
        {
            _dbService = dbService;
        }
        public async Task Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
        {
            await _dbService.DeleteAppointment(request.Id);
        }
    }
}
