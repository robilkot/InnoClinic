using AppointmentsService.Commands;
using AppointmentsService.Data.Models;
using AppointmentsService.Interfaces;
using AppointmentsService.Queries;
using AppointmentsService.Services;
using MediatR;

namespace AppointmentsService.Handlers
{
    public class AddAppointmentHandler : IRequestHandler<AddAppointmentCommand, DbAppointment>
    {
        private readonly IDbService _dbService;
        public AddAppointmentHandler(IDbService dbService)
        {
            _dbService = dbService;
        }
        public async Task<DbAppointment> Handle(AddAppointmentCommand request, CancellationToken cancellationToken)
        {
            var result = await _dbService.AddAppointment(request.Appointment);

            return result;
        }
    }
}
