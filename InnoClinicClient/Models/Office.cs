namespace InnoClinicClient.Models
{
    public class Office
    {
        public Guid? Id { get; set; }
        public required string Address { get; set; }
        public required string RegistryPhoneNumber { get; set; }
        public bool Active { get; set; }
        public Guid? ImageId { get; set; }
    }
}
