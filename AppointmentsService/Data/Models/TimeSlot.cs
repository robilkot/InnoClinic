namespace AppointmentsService.Data.Models
{
    public class TimeSlot
    {
        public DateTime Time { get; set; }
        public TimeSpan Duration { get; set; }
        public bool Occupied { get; set; }
    }
}
