using Kata.Data.Entities;
using System.Data;
using System.Text.Json;

namespace Kata.Data.Services
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly string _directory = "_data";
        private readonly string _appointmentsFilename = "appointments.json";
        private readonly string _timeSlotsFilename = "timeslots.json";
        private readonly string _appointmentsFilePath;
        private readonly string _timeSlotsFilePath;

        public AppointmentRepository()
        {
            _appointmentsFilePath = $"{_directory}/{_appointmentsFilename}";
            _timeSlotsFilePath = $"{_directory}/{_timeSlotsFilename}";

            InitStorage();
        }

        private async Task InitStorage()
        {
            if (!Directory.Exists(_directory))
                Directory.CreateDirectory(_directory);

            // Create Time slots referential
            if (!File.Exists(_timeSlotsFilePath))
            {
                var timeSlots = CreateTimeSlots();
                await SaveTimeSlots(timeSlots);
            }

        }
        private List<TimeSlot> CreateTimeSlots()
        {
            TimeSpan initialStartTime = TimeSpan.FromHours(8); // Starting at 8:00 AM
            TimeSpan interval = TimeSpan.FromMinutes(30); // Interval 30 minutes

            List<TimeSlot> timeSlots = Enumerable.Range(0, 10)
                .Select(i =>
                {
                    TimeSpan startTime = initialStartTime + (i * interval);
                    TimeSpan endTime = startTime + interval;
                    return new TimeSlot { Id = Guid.NewGuid(), StartTime = startTime, EndTime = endTime };
                }).ToList();

            return timeSlots;
        }
        public async Task<IEnumerable<Appointment>> GetAllAppointments()
        {
            var res = await GetAllRecords<Appointment>(_appointmentsFilePath);
            return res.OrderByDescending(a => a.CreatedAt);
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsByDateAndRoom(DateTimeOffset date, int roomNumber)
        {
            var res = (await GetAllAppointments()).Where(app => app.AppointmentDate == date && app.RoomNumber == roomNumber);
            return res;
        }

        public async Task<IEnumerable<TimeSlot>> GetAllTimeSlots()
        {
            var jsonData = await File.ReadAllTextAsync(_timeSlotsFilePath);
            return JsonSerializer.Deserialize<List<TimeSlot>>(jsonData);
        }

        public async Task<Appointment?> GetAppointmentById(Guid id)
        {
            var appointments = await GetAllAppointments();
            return appointments.FirstOrDefault(a => a.Id == id);
        }

        public async Task<Appointment> Add(Appointment appointment)
        {
            var appointments = (await GetAllAppointments()).ToList();
            appointment.Id = Guid.NewGuid();
            appointment.CreatedAt = DateTimeOffset.Now;
            appointments.Add(appointment);
            await SaveAppointments(appointments);
            return appointment;
        }

        public async Task Delete(Guid id)
        {
            var appointments = (await GetAllAppointments()).Where(a => a.Id != id).ToList();
            await SaveAppointments(appointments);
        }

        public async Task<Appointment?> GetAppointmentByDateAndSlotId(DateTimeOffset appointmentDate, Guid timeSlotId, int roomNumber)
        {
            var appointments = await GetAllAppointments();

            return appointments.FirstOrDefault(a => a.AppointmentDate == appointmentDate && a.TimeSlotId == timeSlotId && a.RoomNumber == roomNumber);
        }

        private async Task SaveAppointments(List<Appointment> appointments)
        {
            await SaveRecords(appointments, _appointmentsFilePath);
        }
        private async Task SaveTimeSlots(List<TimeSlot> timeSlots)
        {
            await SaveRecords(timeSlots, _timeSlotsFilePath);
        }

        private async Task SaveRecords<T>(List<T> records, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var jsonData = JsonSerializer.Serialize(records, options);
            await File.WriteAllTextAsync(filePath, jsonData);
        }


        private async Task<IEnumerable<T>> GetAllRecords<T>(string path)
        {
            if (!File.Exists(path))
                return new List<T>();

            var jsonData = await File.ReadAllTextAsync(path);
            var res = JsonSerializer.Deserialize<List<T>>(jsonData);

            if (res == null)
                return new List<T>();

            return res;
        }

    }
}
