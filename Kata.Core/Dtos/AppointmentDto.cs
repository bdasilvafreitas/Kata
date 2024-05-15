using System.ComponentModel.DataAnnotations;

namespace Kata.Core.Dtos
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }

        [Required]
        public required Guid TimeSlotId { get; set; }

        public TimeSlotDto? TimeSlot { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "RoomNumber must be either 1 or 2.")]
        public required int RoomNumber { get; set; }

        [Required]
        public required DateTimeOffset AppointmentDate { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        public required DateTimeOffset DateOfBirth { get; set; }

        [Required]
        public required string Email { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}

