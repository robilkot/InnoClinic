namespace AppointmentsService.Data.Models
{
    public class DbAppointment
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        // Patient's data is redundant
        public required string PatientFirstName { get; set; }
        public string? PatientMiddleName { get; set; }
        public required string PatientLastName { get; set; }
        public Guid DoctorId { get; set; }
        // Doctor's data is redudant
        public required string DoctorFirstName { get; set; }
        public required string DoctorMiddleName { get; set; }
        public required string DoctorLastName { get; set; }
        public Guid ServiceId { get; set; }
        // ServiceName is redundant
        public required string ServiceName { get; set; }
        public Guid OfficeId { get; set; }
        // OfficeAddress is redundant
        public required string OfficeAddress { get; set; }
        public DateTime Date { get; set; }
        public bool IsApproved { get; set; }
    }
}
