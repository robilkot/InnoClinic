using System.ComponentModel.DataAnnotations;
using OfficesService.Data.Models;

namespace OfficesService.Models
{
    public class ClientOfficeModel
    {
        public Guid? Id { get; set; }

        [Required]
        public required string Address { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public required string RegistryPhoneNumber { get; set; }

        [Required]
        public bool Active { get; set; }
        public Guid? ImageId { get; set; }
        public byte[]? Image { get; set; }
    }
}
