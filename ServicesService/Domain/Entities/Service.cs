using Microsoft.EntityFrameworkCore;

namespace ServicesService.Domain.Entities
{
    [PrimaryKey("Id")]
    public class Service
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid? CategoryId { get; set; }
        // slot size is redundant
        public int TimeSlotSize { get; set; }
        public decimal Price { get; set; }
        public Guid? SpecializationId { get; set; }
        public bool IsActive { get; set; }
    }
}
