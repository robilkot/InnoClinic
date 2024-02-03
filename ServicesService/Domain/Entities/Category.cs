using Microsoft.EntityFrameworkCore;

namespace ServicesService.Domain.Entities
{
    [PrimaryKey("Id")]
    public class Category
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public int TimeSlotSize { get; set; }
    }
}
