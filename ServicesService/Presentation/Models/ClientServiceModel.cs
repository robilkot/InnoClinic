using System.ComponentModel.DataAnnotations;

namespace ServicesService.Presentation.Models
{
    public class ClientServiceModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public Guid? CategoryId { get; set; }
        // slot size is redundant
        [Range(1,3)]
        public int? TimeSlotSize { get; set; }
        public decimal? Price { get; set; }
        public Guid? SpecializationId { get; set; }
        public bool? IsActive { get; set; }
    }
}