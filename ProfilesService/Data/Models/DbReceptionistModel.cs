namespace ProfilesService.Data.Models
{
    public class DbReceptionistModel
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? MiddleName { get; set; }
        public Guid AccountId { get; set; }
        public Guid OfficeId { get; set; }
        // address is redundant
        public required string OfficeAddress { get; set; }
    }
}
