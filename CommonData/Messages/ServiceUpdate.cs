namespace CommonData.Messages
{
    public class ServiceUpdate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public Guid SpecializationId { get; set; }
        public string SpecializationName { get; set; }
        public bool IsActive { get; set; }
    }
}
