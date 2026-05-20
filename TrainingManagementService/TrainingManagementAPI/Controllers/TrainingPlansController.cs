using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainingManagementApplication.DTOs.Requests;
using TrainingManagementApplication.Interfaces;

namespace TrainingManagementAPI.Controllers
{
    [ApiController]
    [Route("api/training-plans")]
    [Authorize]
    public class TrainingPlansController : ControllerBase
    {
        private readonly ITrainingPlanService _trainingPlanService;

        public TrainingPlansController(ITrainingPlanService trainingPlanService)
        {
            _trainingPlanService = trainingPlanService;
        }

        // Svako ulogovan moze da vidi sve planove (sa filterima)
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TrainingPlanFilterRequest filter)
        {
            var plans = await _trainingPlanService.GetAllAsync(filter);
            return Ok(plans);
        }

        // Trener vidi svoje planove
        [HttpGet("mine")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> GetMine()
        {
            var trainerId = GetCurrentUserId();
            if (trainerId == null) return Unauthorized();

            var plans = await _trainingPlanService.GetMyPlansAsync(trainerId.Value);
            return Ok(plans);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var plan = await _trainingPlanService.GetByIdAsync(id);
                return Ok(plan);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Samo treneri kreiraju planove
        [HttpPost]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Create([FromBody] CreateTrainingPlanRequest request)
        {
            try
            {
                var trainerId = GetCurrentUserId();
                if (trainerId == null) return Unauthorized();

                var plan = await _trainingPlanService.CreateAsync(trainerId.Value, request);
                return CreatedAtAction(nameof(GetById), new { id = plan.Id }, plan);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Trener edituje svoj plan
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTrainingPlanRequest request)
        {
            try
            {
                var trainerId = GetCurrentUserId();
                if (trainerId == null) return Unauthorized();

                var plan = await _trainingPlanService.UpdateAsync(id, trainerId.Value, request);
                return Ok(plan);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Trener brise svoj plan
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var trainerId = GetCurrentUserId();
                if (trainerId == null) return Unauthorized();

                await _trainingPlanService.DeleteAsync(id, trainerId.Value);
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

        private Guid? GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }
    }
}
