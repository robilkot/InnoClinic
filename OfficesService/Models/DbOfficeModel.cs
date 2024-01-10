using System.ComponentModel.DataAnnotations.Schema;

namespace OfficesService.Models
{
    public class DbOfficeModel
    {
        public Guid Id { get; set; }
        public string Adress { get; set; }
        public string RegistryPhoneNumber { get; set; }
        public bool Active { get; set; }
        public Guid? ImageId { get; set; }

        [ForeignKey("ImageId")]
        public virtual DbImageModel? Image { get; set; }
    }
}
