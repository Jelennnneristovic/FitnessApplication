using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserServiceApplication.DTOs.Requests;
using UserServiceApplication.Interfaces;

namespace UserServiceAPI.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // Klijent ocenjuje trenera
        [HttpPost("trainers/{trainerId:guid}/reviews")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CreateReview(Guid trainerId, [FromBody] CreateReviewRequest request)
        {
            try
            {
                var clientId = GetCurrentUserId();
                if (clientId == null) return Unauthorized();

                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var review = await _reviewService.CreateAsync(trainerId, clientId.Value, token, request);
                return Created($"/api/trainers/{trainerId}/reviews/{review.Id}", review);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Sve ocene trenera (svako moze da vidi)
        [HttpGet("trainers/{trainerId:guid}/reviews")]
        public async Task<IActionResult> GetTrainerReviews(Guid trainerId)
        {
            var reviews = await _reviewService.GetTrainerReviewsAsync(trainerId);
            return Ok(reviews);
        }

        // Prosecna ocena trenera
        [HttpGet("trainers/{trainerId:guid}/rating")]
        public async Task<IActionResult> GetTrainerRating(Guid trainerId)
        {
            var rating = await _reviewService.GetTrainerRatingAsync(trainerId);
            return Ok(rating);
        }

        // Moje ostavljene ocene
        [HttpGet("reviews/mine")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetMyReviews()
        {
            var clientId = GetCurrentUserId();
            if (clientId == null) return Unauthorized();

            var reviews = await _reviewService.GetMyReviewsAsync(clientId.Value);
            return Ok(reviews);
        }

        // Klijent brise svoju ocenu
        [HttpDelete("reviews/{reviewId:guid}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            try
            {
                var clientId = GetCurrentUserId();
                if (clientId == null) return Unauthorized();

                await _reviewService.DeleteAsync(reviewId, clientId.Value);
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
