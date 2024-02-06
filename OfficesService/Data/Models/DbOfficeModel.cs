using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficesService.Data.Models
{
    [Table("Offices")]
    public class DbOfficeModel
    {
        [Key]
        public Guid Id { get; set; }
        public required string Address { get; set; }
        public required string RegistryPhoneNumber { get; set; }
        public bool Active { get; set; }
        public Guid? ImageId { get; set; }
    }
}
