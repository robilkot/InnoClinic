namespace CommonData.Messages
{
    public class PatientUpdate
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? MiddleName { get; set; }
        public bool LinkedToAccount { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Guid AccountId { get; set; }
    }
}
