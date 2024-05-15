using AutoMapper;
using Kata.Core.Dtos;
using Kata.Data.Entities;
using Kata.Data.Services;

namespace Kata.Core.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IMapper _mapper;
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IMapper mapper, IAppointmentRepository appointmentRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        }

        public async Task<AppointmentDto> Create(AppointmentDto appointmentDto)
        {
            var existingAppointment = await _appointmentRepository.GetAppointmentByDateAndSlotId(
                appointmentDto.AppointmentDate,
                appointmentDto.TimeSlotId,
                appointmentDto.RoomNumber);
            if (existingAppointment != null)
            {
                throw new Exception("This slot is unavailable");
            }

            var appointment = new Appointment
            {
                TimeSlotId = appointmentDto.TimeSlotId,
                RoomNumber = appointmentDto.RoomNumber,
                AppointmentDate = appointmentDto.AppointmentDate,
                FirstName = appointmentDto.FirstName,
                LastName = appointmentDto.LastName,
                DateOfBirth = appointmentDto.DateOfBirth,
                Email = appointmentDto.Email
            };

            var result = await _appointmentRepository.Add(appointment);

            return _mapper.Map<AppointmentDto>(result);
        }

        public async Task Delete(Guid id)
        {
            await _appointmentRepository.Delete(id);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAppointments()
        {
            var appointments = await _appointmentRepository.GetAllAppointments();
            var res = _mapper.Map<List<AppointmentDto>>(appointments);

            // Mapping of TimeSlots because no relational database
            var timeSlots = await GetAllTimeSlots();
            res.ForEach(a => a.TimeSlot = timeSlots.FirstOrDefault(t => t.Id == a.TimeSlotId));
            return res;
        }

        public async Task<IEnumerable<TimeSlotDto>> GetAllTimeSlots()
        {
            var timeSlots = await _appointmentRepository.GetAllTimeSlots();
            return _mapper.Map<IEnumerable<TimeSlotDto>>(timeSlots);
        }

        public async Task<IEnumerable<TimeSlotDto>> GetAvailableTimeSlots(DateTimeOffset date, int roomNumber)
        {
            if (date == DateTimeOffset.MinValue || date <= DateTime.Today)
            {
                throw new ArgumentException("Invalid date. Date must be a valid future date.");
            }

            if (roomNumber < 1 || roomNumber > 2)
            {
                throw new ArgumentException("Invalid room number. Room number must be 1 or 2.");
            }

            var allTimeSlots = await _appointmentRepository.GetAllTimeSlots();
            var appointments = await _appointmentRepository.GetAllAppointmentsByDateAndRoom(date, roomNumber);

            var availableTimeSlots = allTimeSlots.Where(t => !appointments.Any(a => a.TimeSlotId == t.Id));
            return _mapper.Map<IEnumerable<TimeSlotDto>>(availableTimeSlots);
        }


        public async Task<AppointmentDto> GetById(Guid id)
        {
            var appointment = await _appointmentRepository.GetAppointmentById(id);
            return _mapper.Map<AppointmentDto>(appointment);
        }
    }
}
