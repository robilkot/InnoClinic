using System.ComponentModel.DataAnnotations;

namespace OfficesService.Models
{
    public class ClientOfficeModel
    {
        public Guid? Id { get; set; }

        [Required]
        // todo:  implement validator
        public string Adress { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string RegistryPhoneNumber { get; set; }

        [Required]
        public bool Active { get; set; }
        public DbImageModel? Image { get; set; }
    }
}
