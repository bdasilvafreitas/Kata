using Kata.Data.Entities;

namespace Kata.Data.Services
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAppointments();
        Task<IEnumerable<Appointment>> GetAllAppointmentsByDateAndRoom(DateTimeOffset date, int roomNumber);
        Task<IEnumerable<TimeSlot>> GetAllTimeSlots();
        Task<Appointment?> GetAppointmentById(Guid id);
        Task<Appointment?> GetAppointmentByDateAndSlotId(DateTimeOffset appointmentDate, Guid timeSlotId, int roomNumber);
        Task<Appointment> Add(Appointment appointment);
        Task Delete(Guid id);

    }
}