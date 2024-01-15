namespace ProfilesService.Models
{
    public class DbDoctorModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Guid AccountId { get; set; }
        public Guid SpecializationId { get; set; }
        // specname is redundant
        public string SpecializationName { get; set; }
        public Guid OfficeId { get; set; }
        // address is redundant
        public string OfficeAddress { get; set; }
        public DateTime CareerStart { get; set; }
        public DoctorStatusEnum Status { get; set; }
    }
}
