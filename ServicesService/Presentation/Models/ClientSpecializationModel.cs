namespace ServicesService.Domain.Entities
{
    public class ClientSpecializationModel
    {
        public Guid? Id { get; set; }
        public required string? Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
