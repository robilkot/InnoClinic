using AppointmentsService.Commands;
using AppointmentsService.Data.Models;
using AppointmentsService.Queries;
using AppointmentsService.Services;
using MediatR;

namespace AppointmentsService.Handlers
{
    public class UpdateAppointmentHandler : IRequestHandler<UpdateAppointmentCommand, DbAppointment>
    {
        private readonly DbService _dbService;
        public UpdateAppointmentHandler(DbService dbService)
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
