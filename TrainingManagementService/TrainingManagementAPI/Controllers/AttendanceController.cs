using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainingManagementApplication.DTOs.Requests;
using TrainingManagementApplication.Interfaces;

namespace TrainingManagementAPI.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // Klijent oznacava da je sam dosao na sesiju
        [HttpPost("sessions/{sessionId:guid}/mark-mine")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> MarkMine(Guid sessionId, [FromBody] MarkAttendanceRequest request)
        {
            try
            {
                var clientId = GetCurrentUserId();
                if (clientId == null) return Unauthorized();

                var attendance = await _attendanceService.MarkAsync(sessionId, clientId.Value, clientId.Value, request);
                return Ok(attendance);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Trener oznacava dolazak nekog klijenta
        [HttpPost("sessions/{sessionId:guid}/clients/{clientId:guid}/mark")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> MarkByTrainer(Guid sessionId, Guid clientId, [FromBody] MarkAttendanceRequest request)
        {
            try
            {
                var trainerId = GetCurrentUserId();
                if (trainerId == null) return Unauthorized();

                var attendance = await _attendanceService.MarkAsync(sessionId, clientId, trainerId.Value, request);
                return Ok(attendance);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Klijent vidi svoju istoriju
        [HttpGet("my-history")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetMyHistory()
        {
            var clientId = GetCurrentUserId();
            if (clientId == null) return Unauthorized();

            var history = await _attendanceService.GetMyHistoryAsync(clientId.Value);
            return Ok(history);
        }

        // Trener vidi listu dolazaka za sesiju
        [HttpGet("sessions/{sessionId:guid}")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> GetBySession(Guid sessionId)
        {
            try
            {
                var trainerId = GetCurrentUserId();
                if (trainerId == null) return Unauthorized();

                var attendances = await _attendanceService.GetBySessionAsync(sessionId, trainerId.Value);
                return Ok(attendances);
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

        private Guid? GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }
    }
}
