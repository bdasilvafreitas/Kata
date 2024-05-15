namespace Kata.Data.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid TimeSlotId { get; set; }
        public DateTimeOffset AppointmentDate { get; set; }
        public int RoomNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Email { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
