using AppointmentsService.Data.Models;
using AppointmentsService.Queries;
using AppointmentsService.Services;
using MediatR;

namespace AppointmentsService.Handlers
{
    public class GetAppointmentByIdHandler : IRequestHandler<GetAppointmentByIdQuery, DbAppointment>
    {
        private readonly DbService _dbService;
        public GetAppointmentByIdHandler(DbService dbService)
        {
            _dbService = dbService;
        }
        public async Task<DbAppointment> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbService.GetAppointment(request.Id);

            return result;
        }
    }
}
