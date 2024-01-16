using System.ComponentModel.DataAnnotations;

namespace ProfilesService.Models
{
    public class ClientReceptionistModel
    {
        public Guid? Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public Guid? AccountId { get; set; }
        public Guid OfficeId { get; set; }
        // address is redundant
        public string OfficeAddress { get; set; }
    }
}
