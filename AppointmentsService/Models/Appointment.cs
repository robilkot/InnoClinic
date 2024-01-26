namespace AppointmentsService.Models
{
    public class Appointment
    {
        public Guid Id {  get; set; }
        public Guid PatientId { get; set; }
        // Patient's name is redundant
        public string PatientFirstName {  get; set; }
        public string PatientMiddleName { get; set; }
        public string PatientLastName { get; set; }
        public Guid DoctorId { get; set; }
        // Doctor's name is redudant
        public string DoctorFirstName { get; set; }
        public string DoctorMiddleName { get; set; }
        public string DoctorLastName { get; set; }
        public Guid ServiceId { get; set; }
        // ServiceName is redundant
        public string ServiceName { get; set; }
        public DateTime? Date {  get; set; }
        public bool IsApproved {  get; set; }
    }
}
