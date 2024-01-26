using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficesService.Data.Models
{
    [Table("Offices")]
    public class DbOfficeModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Adress { get; set; }
        public string RegistryPhoneNumber { get; set; }
        public bool Active { get; set; }
        public Guid? ImageId { get; set; }
        // Image is redundant
        public byte[]? Image {  get; set; }
    }
}
