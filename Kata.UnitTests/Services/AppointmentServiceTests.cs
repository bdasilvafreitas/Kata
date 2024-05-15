using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Kata.Core.Dtos;
using Kata.Core.Services;
using Kata.Data.Entities;
using Kata.Data.Services;
using Moq;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kata.UnitTests.Services
{
    public class AppointmentServiceTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IAppointmentRepository> _mockAppointmentRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AppointmentService _appointmentService;

        public AppointmentServiceTests()
        {
            _fixture = new Fixture();
            _mockAppointmentRepository = new Mock<IAppointmentRepository>();
            _mockMapper = new Mock<IMapper>();
            _appointmentService = new AppointmentService(_mockMapper.Object, _mockAppointmentRepository.Object);
        }

        [Fact]
        public async Task Create_ThrowsExceptionForExistingAppointment()
        {
            // Arrange      
            var appointmentDto = _fixture.Create<AppointmentDto>();

            _mockAppointmentRepository.Setup(repo => repo.GetAppointmentByDateAndSlotId(
                appointmentDto.AppointmentDate,
                appointmentDto.TimeSlotId,
                appointmentDto.RoomNumber))
                .ReturnsAsync(new Appointment());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _appointmentService.Create(appointmentDto));
            _mockAppointmentRepository.Verify(repo => repo.Add(It.IsAny<Appointment>()), Times.Never);
        }

        [Fact]
        public async Task Create_MapsAndSavesNewAppointment()
        {
            // Arrange
            var appointmentDto = _fixture.Create<AppointmentDto>();
            var appointment = _fixture.Build<Appointment>()
                .With(a => a.Id, appointmentDto.Id)
                .Create();
            var mappedDto = appointmentDto;
            _mockAppointmentRepository.Setup(repo => repo.GetAppointmentByDateAndSlotId(appointmentDto.AppointmentDate, appointmentDto.TimeSlotId, appointmentDto.RoomNumber))
                .ReturnsAsync((Appointment)null);
            _mockAppointmentRepository.Setup(repo => repo.Add(It.IsAny<Appointment>()))
                .ReturnsAsync(appointment);
            _mockMapper.Setup(mapper => mapper.Map<Appointment>(appointmentDto))
                .Returns(appointment);
            _mockMapper.Setup(mapper => mapper.Map<AppointmentDto>(appointment))
                .Returns(mappedDto);

            // Act
            var result = await _appointmentService.Create(appointmentDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mappedDto.Id, result.Id);
            _mockMapper.Verify(mapper => mapper.Map<AppointmentDto>(appointment), Times.Once);

        }

        [Fact]
        public async Task Delete_CallsRepositoryDelete()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();

            // Act
            await _appointmentService.Delete(appointmentId);

            // Assert
            _mockAppointmentRepository.Verify(repo => repo.Delete(appointmentId), Times.Once);
        }


        [Fact]
        public async Task GetAvailableTimeSlots_ThrowsExceptionForInvalidDate()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _appointmentService.GetAvailableTimeSlots(DateTimeOffset.MinValue, 1));
        }

        [Fact]
        public async Task GetAvailableTimeSlots_WhenAllTimeSlotsAreAvailable()
        {
            // Arrange
            var timeSlots = _fixture.Create<List<TimeSlot>>();
            var mappedTimeSlots = _fixture.Create<List<TimeSlotDto>>();
            var appointments = _fixture.Create<List<Appointment>>();
            var appointmentDate = DateTimeOffset.Now;
            var roomNumber = 2;

            _mockAppointmentRepository.Setup(repo => repo.GetAllTimeSlots())
                .ReturnsAsync(timeSlots);
            _mockAppointmentRepository.Setup(repo => repo.GetAllAppointmentsByDateAndRoom(appointmentDate, roomNumber))
                .ReturnsAsync(appointments);
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TimeSlotDto>>(timeSlots))
                .Returns(mappedTimeSlots);

            // Act
            var res = await _appointmentService.GetAvailableTimeSlots(appointmentDate, roomNumber);

            // Assert
            Assert.NotEmpty(res);
            Assert.Equal(res.Count(), timeSlots.Count);
            _mockAppointmentRepository.Verify(repo => repo.GetAllTimeSlots(), Times.Once);
            _mockAppointmentRepository.Verify(repo => repo.GetAllAppointmentsByDateAndRoom(appointmentDate, roomNumber), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map<IEnumerable<TimeSlotDto>>(timeSlots), Times.Once);
        }

        [Fact]
        public async Task GetAvailableTimeSlots_WithUnvailableTimeSlots()
        {
            // Arrange
            var timeSlots = _fixture.Create<List<TimeSlot>>();
            var mappedTimeSlots = _fixture.Create<List<TimeSlotDto>>();
            var appointmentsByDateAndRoom = _fixture.Create<List<Appointment>>();
            var BookedTimeSlotAppointment = _fixture.Build<Appointment>().With(x => x.TimeSlotId, timeSlots[0].Id).Create();
            appointmentsByDateAndRoom.Add(BookedTimeSlotAppointment);
            var appointmentDate = DateTimeOffset.Now;
            var roomNumber = 2;

            _mockAppointmentRepository.Setup(repo => repo.GetAllTimeSlots())
                .ReturnsAsync(timeSlots);
            _mockAppointmentRepository.Setup(repo => repo.GetAllAppointmentsByDateAndRoom(appointmentDate, roomNumber))
                .ReturnsAsync(appointmentsByDateAndRoom);
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TimeSlotDto>>(It.IsAny<IEnumerable<TimeSlot>>()))
                .Returns(mappedTimeSlots);

            // Act
            var res = await _appointmentService.GetAvailableTimeSlots(appointmentDate, roomNumber);

            // Assert
            Assert.NotEmpty(res);
            Assert.Equal(timeSlots.Count, res.Count()); 
            _mockAppointmentRepository.Verify(repo => repo.GetAllTimeSlots(), Times.Once);
            _mockAppointmentRepository.Verify(repo => repo.GetAllAppointmentsByDateAndRoom(appointmentDate, roomNumber), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map<IEnumerable<TimeSlotDto>>(It.IsAny<IEnumerable<TimeSlot>>()), Times.Once);
        }



        [Fact]
        public async Task GetAppointmentById_ReturnsFoundAppointment()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var appointment = _fixture.Build<Appointment>().With(t => t.Id, id).Create();
            var mappedDto = _fixture.Build<AppointmentDto>().With(t => t.Id, id).Create();
            _mockAppointmentRepository.Setup(repo => repo.GetAppointmentById(id))
                .ReturnsAsync(appointment);
            _mockMapper.Setup(mapper => mapper.Map<AppointmentDto>(appointment))
                .Returns(mappedDto);
            // Act
            var res = await _appointmentService.GetById(id);

            // Assert
            Assert.Equal(id, res.Id);
            _mockAppointmentRepository.Verify(repo => repo.GetAppointmentById(id), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map<AppointmentDto>(appointment), Times.Once);
        }


    }
}
