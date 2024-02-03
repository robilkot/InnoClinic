using System.ComponentModel.DataAnnotations;

namespace ServicesService.Domain.Entities
{
    public class ClientCategoryModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        [Range(1, 3)]
        public int? TimeSlotSize { get; set; }
    }
}
