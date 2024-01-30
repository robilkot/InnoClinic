using AppointmentsService.Data.Models;
using MediatR;

namespace AppointmentsService.Queries
{
    public class GetAppointmentsQuery : IRequest<IEnumerable<DbAppointment>>
    {
        public int PageNumber {  get; set; }
        public int PageSize { get; set; }
        public DateTime? Date {  get; set; }
        public Guid? DoctorId { get; set; }
        public Guid? ServiceId {  get; set; }
        public bool? Approved { get; set; }
        public Guid? OfficeId {  get; set; }
        public Guid? PatientId { get; set; }
    }
}
