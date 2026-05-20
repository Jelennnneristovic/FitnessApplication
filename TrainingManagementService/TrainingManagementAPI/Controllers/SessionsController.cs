using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainingManagementApplication.DTOs.Requests;
using TrainingManagementApplication.Interfaces;

namespace TrainingManagementAPI.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    [Authorize]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // Trener kreira sesiju
        [HttpPost]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Create([FromBody] CreateSessionRequest request)
        {
            try
            {
                var trainerId = GetCurrentUserId();
                if (trainerId == null) return Unauthorized();

                var session = await _sessionService.CreateAsync(trainerId.Value, request);
                return Created($"/api/sessions/{session.Id}", session);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Trener edituje svoju sesiju
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSessionRequest request)
        {
            try
            {
                var trainerId = GetCurrentUserId();
                if (trainerId == null) return Unauthorized();

                var session = await _sessionService.UpdateAsync(id, trainerId.Value, request);
                return Ok(session);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Trener brise sesiju
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var trainerId = GetCurrentUserId();
                if (trainerId == null) return Unauthorized();

                await _sessionService.DeleteAsync(id, trainerId.Value);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // Detalji jedne sesije
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var session = await _sessionService.GetByIdAsync(id);
                return Ok(session);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Sesije za konkretan plan (svako ulogovan moze)
        [HttpGet("by-plan/{planId:guid}")]
        public async Task<IActionResult> GetByPlan(Guid planId)
        {
            var sessions = await _sessionService.GetByPlanAsync(planId);
            return Ok(sessions);
        }

        // Trenerov kalendar
        [HttpGet("trainer-schedule")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> GetTrainerSchedule(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var trainerId = GetCurrentUserId();
            if (trainerId == null) return Unauthorized();

            var sessions = await _sessionService.GetMyTrainerScheduleAsync(trainerId.Value, from, to);
            return Ok(sessions);
        }

        // Klijentov kalendar (sve sesije iz planova na koje je Approved)
        [HttpGet("client-schedule")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetClientSchedule(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var clientId = GetCurrentUserId();
            if (clientId == null) return Unauthorized();

            var sessions = await _sessionService.GetMyClientScheduleAsync(clientId.Value, from, to);
            return Ok(sessions);
        }

        private Guid? GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }
    }
}
