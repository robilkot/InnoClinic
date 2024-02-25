namespace InnoClinicClient.Models
{
    public class Appointment
    {
        public Guid? Id { get; set; }
        public Guid PatientId { get; set; }
        public string? PatientFirstName { get; set; }
        public string? PatientMiddleName { get; set; }
        public string? PatientLastName { get; set; }
        public Guid DoctorId { get; set; }
        public string? DoctorFirstName { get; set; }
        public string? DoctorMiddleName { get; set; }
        public string? DoctorLastName { get; set; }
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public int? TimeSlotSize { get; set; }
        public Guid OfficeId { get; set; }
        public string? OfficeAddress { get; set; }
        public DateTime Date { get; set; }
        public bool? IsApproved { get; set; }
    }
}
