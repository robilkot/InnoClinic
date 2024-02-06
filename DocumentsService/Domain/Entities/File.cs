namespace DocumentsService.Domain.Entities
{
    public class File
    {
        public Guid Id { get; set; }
        public Guid? OwnerId { get; set; }
        public required string Name { get; set; }
        public required byte[] Content { get; set; }
    }
}
