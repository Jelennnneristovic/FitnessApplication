using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrainingManagementApplication.Interfaces;

namespace TrainingManagementAPI.Controllers
{
    [ApiController]
    [Route("api/internal")]
    [Authorize]  // i dalje zahteva validan token
    public class InternalController : ControllerBase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public InternalController(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        // Proverava da li je klijent trenirao sa trenerom
        [HttpGet("has-trained")]
        public async Task<IActionResult> HasTrained(
            [FromQuery] Guid clientId,
            [FromQuery] Guid trainerId)
        {
            if (clientId == Guid.Empty || trainerId == Guid.Empty)
                return BadRequest(new { message = "clientId i trainerId su obavezni." });

            var hasTrained = await _enrollmentRepository
                .HasApprovedEnrollmentWithTrainerAsync(clientId, trainerId);

            return Ok(new { hasTrained });
        }
    }
}
