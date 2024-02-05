namespace AppointmentsService.Models
{
    public class ClientTimeSlot
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public bool Occupied { get; set; }
        public Guid? AppointmentId { get; set; }
    }
}
