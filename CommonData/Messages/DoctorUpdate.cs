using CommonData.enums;

namespace CommonData.Messages
{
    public class DoctorUpdate
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Guid AccountId { get; set; }
        public Guid SpecializationId { get; set; }
        // specname is redundant
        public required string SpecializationName { get; set; }
        public Guid OfficeId { get; set; }
        // address is redundant
        public required string OfficeAddress { get; set; }
        public DateTime CareerStart { get; set; }
        public DoctorStatusEnum Status { get; set; }
    }
}
