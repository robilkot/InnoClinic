using Microsoft.EntityFrameworkCore;

namespace ServicesService.Domain.Entities
{
    [PrimaryKey("Id")]
    public class Specialization
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
