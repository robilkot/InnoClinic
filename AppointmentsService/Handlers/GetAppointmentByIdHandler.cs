using AppointmentsService.Data.Models;
using AppointmentsService.Interfaces;
using AppointmentsService.Queries;
using MediatR;

namespace AppointmentsService.Handlers
{
    public class GetAppointmentByIdHandler : IRequestHandler<GetAppointmentByIdQuery, DbAppointment>
    {
        private readonly IDbService _dbService;
        public GetAppointmentByIdHandler(IDbService dbService)
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
