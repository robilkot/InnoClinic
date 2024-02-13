namespace InnoClinicClient.Models
{
    public class Patient
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public bool? LinkedToAccount { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Guid? AccountId { get; set; }
    }
}
