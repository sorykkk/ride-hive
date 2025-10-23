using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideHiveApi.Data;
using RideHiveApi.Models;
using RideHiveApi.Models.DataTransferObjects;
using RideHiveApi.Models.Enums;

namespace RideHiveApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(AppDbContext context, ILogger<NotificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Notifications?userId={userId}
        // Get all notifications for a specific user
        [HttpGet]
        public async Task<ActionResult<List<NotificationResponseDto>>> GetNotifications([FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("UserId is required");
                }

                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                var notificationDtos = new List<NotificationResponseDto>();

                foreach (var notification in notifications)
                {
                    // Get request info
                    var request = await _context.Requests.FindAsync(notification.RequestId);
                    if (request == null) continue;

                    // Skip booking_request notifications if request is no longer Pending
                    if (notification.Type == "booking_request" && request.Status != RequestStatus.Pending)
                    {
                        // This notification is stale - the request was already processed
                        continue;
                    }

                    // Get post and car info
                    var post = await _context.PostItems.FindAsync(notification.PostId);
                    var car = post != null ? await _context.CarItems.FindAsync(post.CarId) : null;

                    // Determine who the "related user" is based on notification type
                    AppUser? relatedUser = null;
                    if (notification.Type == "booking_request")
                    {
                        // For owner notifications, related user is the client
                        relatedUser = await _context.Users.FindAsync(request.UserId);
                    }
                    else if (notification.Type == "booking_accepted" || notification.Type == "booking_rejected")
                    {
                        // For client notifications, related user is the owner
                        if (post != null)
                        {
                            relatedUser = await _context.Users.FindAsync(post.OwnerId);
                        }
                    }

                    var dto = NotificationResponseDto.FromNotification(
                        notification,
                        request,
                        relatedUser,
                        post,
                        car
                    );

                    notificationDtos.Add(dto);
                }

                return Ok(notificationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching notifications for user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Notifications/{id}
        // Get a specific notification by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationResponseDto>> GetNotification(int id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);

                if (notification == null)
                {
                    return NotFound($"Notification with ID {id} not found");
                }

                // Get related data
                var request = await _context.Requests.FindAsync(notification.RequestId);
                var post = await _context.PostItems.FindAsync(notification.PostId);
                var car = post != null ? await _context.CarItems.FindAsync(post.CarId) : null;

                AppUser? relatedUser = null;
                if (notification.Type == "booking_request" && request != null)
                {
                    relatedUser = await _context.Users.FindAsync(request.UserId);
                }
                else if ((notification.Type == "booking_accepted" || notification.Type == "booking_rejected") && post != null)
                {
                    relatedUser = await _context.Users.FindAsync(post.OwnerId);
                }

                var dto = NotificationResponseDto.FromNotification(
                    notification,
                    request,
                    relatedUser,
                    post,
                    car
                );

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching notification {NotificationId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Notifications/{id}/read
        // Mark a notification as read
        [HttpPut("{id}/read")]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);

                if (notification == null)
                {
                    return NotFound($"Notification with ID {id} not found");
                }

                notification.IsRead = true;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Notification marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Notifications/read-all?userId={userId}
        // Mark all notifications as read for a user
        [HttpPut("read-all")]
        public async Task<ActionResult> MarkAllAsRead([FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("UserId is required");
                }

                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = $"Marked {notifications.Count} notifications as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Notifications/{id}
        // Delete a notification
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNotification(int id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);

                if (notification == null)
                {
                    return NotFound($"Notification with ID {id} not found");
                }

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Notification deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {NotificationId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Notifications/unread-count?userId={userId}
        // Get count of unread notifications for a user
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount([FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("UserId is required");
                }

                var count = await _context.Notifications
                    .CountAsync(n => n.UserId == userId && !n.IsRead);

                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching unread count for user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
