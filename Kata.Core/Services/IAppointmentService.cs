using Kata.Core.Dtos;

namespace Kata.Core.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAllAppointments();
        Task<IEnumerable<TimeSlotDto>> GetAllTimeSlots();
        Task<IEnumerable<TimeSlotDto>> GetAvailableTimeSlots(DateTimeOffset date, int roomNumber);
        Task<AppointmentDto> GetById(Guid id);
        Task<AppointmentDto> Create(AppointmentDto appointmentDto);
        Task Delete(Guid id);
    }
}
