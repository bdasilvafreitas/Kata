using Kata.Core.Dtos;
using Kata.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Kata.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {

        private readonly ILogger<AppointmentController> _logger;
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(ILogger<AppointmentController> logger, IAppointmentService appointmentService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
        }

        [HttpGet("TimeSlots")]
        [ProducesResponseType(typeof(IEnumerable<TimeSlotDto>), 200)]
        public async Task<ActionResult<IEnumerable<TimeSlotDto>>> GetAllTimeSlots()
        {
            var appointments = await _appointmentService.GetAllTimeSlots();
            return Ok(appointments);
        }


        [HttpGet("AvailableTimeSlots")]
        [ProducesResponseType(typeof(IEnumerable<TimeSlotDto>), 200)]
        public async Task<ActionResult<IEnumerable<TimeSlotDto>>> GetAvailableTimeSlots(DateTimeOffset date, int roomNumber)
        {
            var appointments = await _appointmentService.GetAvailableTimeSlots(date, roomNumber);
            return Ok(appointments);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), 200)]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAllAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointments();
            return Ok(appointments);
        }

        [HttpPost]
        [ProducesResponseType(typeof(AppointmentDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AppointmentDto>> CreateAppointment([FromBody] AppointmentDto appointmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdAppointment = await _appointmentService.Create(appointmentDto);
                return CreatedAtAction(nameof(GetAppointmentById), new { id = createdAppointment.Id }, createdAppointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error has occurred : {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteAppointment(Guid id)
        {

            await _appointmentService.Delete(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AppointmentDto>> GetAppointmentById(Guid id)
        {
            var appointment = await _appointmentService.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }
    }
}
