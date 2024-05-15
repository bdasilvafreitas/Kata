namespace Kata.Data.Entities
{
    public class TimeSlot
    {
        public Guid Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}