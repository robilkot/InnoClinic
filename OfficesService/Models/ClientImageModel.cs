using System.ComponentModel.DataAnnotations;

namespace OfficesService.Models
{
    public class ClientImageModel
    {
        [Required]
        public string Url { get; set; }
    }
}
