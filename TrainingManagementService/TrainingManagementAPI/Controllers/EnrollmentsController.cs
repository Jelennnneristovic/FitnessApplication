using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainingManagementApplication.DTOs.Requests;
using TrainingManagementApplication.Interfaces;
using TrainingManagementDomain.Enums;

namespace TrainingManagementAPI.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // === KLIJENT ===

        // Klijent salje zahtev za prijavu na plan
        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> RequestEnrollment([FromBody] CreateEnrollmentRequest request)
        {
            try
            {
                var clientId = GetCurrentUserId();
                if (clientId == null) return Unauthorized();

                var enrollment = await _enrollmentService.RequestEnrollmentAsync(clientId.Value, request);
                return Created($"/api/enrollments/{enrollment.Id}", enrollment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Klijent vidi svoje zahteve (sa opcionim filterom po statusu)
        [HttpGet("mine")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetMine([FromQuery] EnrollmentStatus? status)
        {
            var clientId = GetCurrentUserId();
            if (clientId == null) return Unauthorized();

            var enrollments = await _enrollmentService.GetMyEnrollmentsAsync(clientId.Value, status);
            return Ok(enrollments);
        }

        // Klijent otkazuje svoj Pending zahtev
        [HttpDelete("{id:guid}/cancel")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                var clientId = GetCurrentUserId();
                if (clientId == null) return Unauthorized();

                await _enrollmentService.CancelMyEnrollmentAsync(id, clientId.Value);
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // === TRENER ===

        // Trener vidi sve zahteve za svoje planove
        [HttpGet("for-my-plans")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> GetForMyPlans([FromQuery] EnrollmentStatus? status)
        {
            var trainerId = GetCurrentUserId();
            if (trainerId == null) return Unauthorized();

            var enrollments = await _enrollmentService.GetEnrollmentsForMyPlansAsync(trainerId.Value, status);
            return Ok(enrollments);
        }

        // Trener vidi zahteve za konkretan plan
        [HttpGet("by-plan/{planId:guid}")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> GetByPlan(Guid planId, [FromQuery] EnrollmentStatus? status)
        {
            try
            {
                var trainerId = GetCurrentUserId();
                if (trainerId == null) return Unauthorized();

                var enrollments = await _enrollmentService.GetEnrollmentsByPlanAsync(planId, trainerId.Value, status);
                return Ok(enrollments);
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

        // Trener odobrava zahtev
        [HttpPatch("{id:guid}/approve")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Approve(Guid id)
        {
            try
            {
                var trainerId = GetCurrentUserId();
                if (trainerId == null) return Unauthorized();

                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var enrollment = await _enrollmentService.ApproveAsync(id, trainerId.Value, token);
                return Ok(enrollment);
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
                return Conflict(new { message = ex.Message });
            }
        }

        //trener odbija zahtev
        [HttpPatch("{id:guid}/reject")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectEnrollmentRequest request)
        {
            try
            {
                var trainerId = GetCurrentUserId();
                if (trainerId == null) return Unauthorized();

                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var enrollment = await _enrollmentService.RejectAsync(id, trainerId.Value, token, request);
                return Ok(enrollment);
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

        // Trener izbacuje odobrenog klijenta iz plana
        [HttpDelete("{id:guid}/remove-client")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> RemoveClient(Guid id)
        {
            try
            {
                var trainerId = GetCurrentUserId();
                if (trainerId == null) return Unauthorized();

                await _enrollmentService.RemoveClientFromPlanAsync(id, trainerId.Value);
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // === HELPER ===

        private Guid? GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }
    }
}
