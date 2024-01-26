namespace ProfilesService.Data.Models
{
    public class DbReceptionistModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public Guid AccountId { get; set; }
        public Guid OfficeId { get; set; }
        // address is redundant
        public string OfficeAddress { get; set; }
    }
}
