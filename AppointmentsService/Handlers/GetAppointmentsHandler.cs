using AppointmentsService.Data.Models;
using AppointmentsService.Queries;
using AppointmentsService.Services;
using MediatR;

namespace AppointmentsService.Handlers
{
    public class GetAppointmentsHandler : IRequestHandler<GetAppointmentsQuery, IEnumerable<DbAppointment>>
    {
        private readonly DbService _dbService;
        public GetAppointmentsHandler(DbService dbService)
        {
            _dbService = dbService;
        }
        public async Task<IEnumerable<DbAppointment>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbService.GetAppointments(request.PageNumber, request.PageSize,
                request.Date, request.DoctorId, request.ServiceId, request.Approved, request.OfficeId, request.PatientId);

            return result;
        }
    }
}
