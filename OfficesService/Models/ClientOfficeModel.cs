using System.ComponentModel.DataAnnotations;

namespace OfficesService.Models
{
    public class ClientOfficeModel
    {
        public Guid? Id { get; set; }

        [Required]
        // todo: validate through regex or smth
        public string Adress { get; set; }

        [Required]
        public string RegistryPhoneNumber { get; set; }

        [Required]
        public bool Active { get; set; }
        public DbImageModel? Image { get; set; }
    }
}
