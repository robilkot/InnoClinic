namespace AppointmentsService.Models
{
    public class ClientAppointment
    {
        public Guid? Id { get; set; }
        public Guid PatientId { get; set; }
        // Patient's data is redundant
        public string? PatientFirstName { get; set; }
        public string? PatientMiddleName { get; set; }
        public string? PatientLastName { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid DoctorId { get; set; }
        // Doctor's name is redudant
        public string? DoctorFirstName { get; set; }
        public string? DoctorMiddleName { get; set; }
        public string? DoctorLastName { get; set; }
        public Guid ServiceId { get; set; }
        // ServiceName is redundant
        public string? ServiceName { get; set; }
        public Guid OfficeId { get; set; }
        public string? OfficeAddress { get; set; }
        public DateTime Date { get; set; }
        public bool? IsApproved { get; set; }
    }
}
