using System.ComponentModel.DataAnnotations;
using OfficesService.Data.Models;

namespace OfficesService.Models
{
    public class ClientOfficeModel
    {
        public Guid? Id { get; set; }

        [Required]
        public string Adress { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string RegistryPhoneNumber { get; set; }

        [Required]
        public bool Active { get; set; }
        public Guid? ImageId { get; set; }
        public byte[]? Image { get; set; }
    }
}
