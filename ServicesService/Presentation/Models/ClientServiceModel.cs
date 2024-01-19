namespace ServicesService.Presentation.Models
{
    public class ClientServiceModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        // Catname is redundant
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public Guid SpecializationId { get; set; }
        // Specname is redundant
        public string? SpecializationName { get; set; }
        public bool IsActive { get; set; }
    }
}