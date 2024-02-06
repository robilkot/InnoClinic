using CommonData.enums;
using System.ComponentModel.DataAnnotations;

namespace ProfilesService.Models
{
    public class ClientDoctorModel
    {
        public Guid? Id { get; set; }
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Guid? AccountId { get; set; }
        public Guid SpecializationId { get; set; }
        // specname is redundant
        public string? SpecializationName { get; set; }
        public Guid OfficeId { get; set; }
        // address is redundant
        public string? OfficeAddress { get; set; }
        public DateTime? CareerStart { get; set; }
        public DoctorStatusEnum? Status { get; set; }
    }
}
