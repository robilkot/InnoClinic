namespace CommonData.Constants
{
    public static class TimeSlotsConstants
    {
        public static readonly List<(int, int)?> WorkingHours = new(){
        null,
        (8,20),
        (8,20),
        (8,20),
        (8,20),
        (8,18),
        null,
        };

        public const int DefaultTimeSlotSize = 2;
        public const int TimeSlotLength = 10;
    }
}
