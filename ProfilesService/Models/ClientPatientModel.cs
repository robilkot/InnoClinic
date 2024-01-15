using System.ComponentModel.DataAnnotations;

namespace ProfilesService.Models
{
    public class ClientPatientModel
    {
        public Guid? Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public bool? LinkedToAccount { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        public Guid? AccountId {  get; set; }
    }
}
