using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotificationServiceApplication.DTOs.Requests;
using NotificationServiceApplication.Interfaces;
using System.Security.Claims;

namespace NotificationServiceAPI.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // Kreira notifikaciju (drugi servisi ili admin)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNotificationRequest request)
        {
            try
            {
                var notification = await _notificationService.CreateAsync(request);
                return Created($"/api/notifications/{notification.Id}", notification);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Moje notifikacije (sve)
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var notifications = await _notificationService.GetMyNotificationsAsync(userId.Value, unreadOnly: false);
            return Ok(notifications);
        }

        // Moje nepročitane notifikacije
        [HttpGet("mine/unread")]
        public async Task<IActionResult> GetMineUnread()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var notifications = await _notificationService.GetMyNotificationsAsync(userId.Value, unreadOnly: true);
            return Ok(notifications);
        }

        // Označi notifikaciju kao pročitanu
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null) return Unauthorized();

                var notification = await _notificationService.MarkAsReadAsync(id, userId.Value);
                return Ok(notification);
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

        // Označi sve kao pročitano
        [HttpPatch("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            await _notificationService.MarkAllAsReadAsync(userId.Value);
            return NoContent();
        }

        private Guid? GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }
    }
}
