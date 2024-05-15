namespace Kata.Core.Dtos
{
    public class TimeSlotDto
    {
        public Guid Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}