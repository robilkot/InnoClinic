using AppointmentsService.Commands;
using AppointmentsService.Data.Models;
using AppointmentsService.Interfaces;
using MediatR;

namespace AppointmentsService.Handlers
{
    public class UpdateAppointmentHandler : IRequestHandler<UpdateAppointmentCommand, DbAppointment>
    {
        private readonly IDbService _dbService;
        public UpdateAppointmentHandler(IDbService dbService)
        {
            _dbService = dbService;
        }
        public async Task<DbAppointment> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var result = await _dbService.UpdateAppointment(request.Appointment);

            return result;
        }
    }
}
