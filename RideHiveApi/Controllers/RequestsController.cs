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
    public class RequestsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RequestsController> _logger;

        public RequestsController(AppDbContext context, ILogger<RequestsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/Requests
        // Create a new booking request
        [HttpPost]
        public async Task<ActionResult<RequestResponseDto>> CreateRequest([FromBody] RequestCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Validate that the User exists
                var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
                if (!userExists)
                {
                    return BadRequest($"User with ID '{dto.UserId}' does not exist");
                }

                // Validate that the Post exists and is available
                var post = await _context.PostItems.FirstOrDefaultAsync(p => p.PostId == dto.PostId);
                if (post == null)
                {
                    return BadRequest($"Post with ID {dto.PostId} does not exist");
                }

                if (!post.Available)
                {
                    return BadRequest($"Post with ID {dto.PostId} is not available for booking");
                }

                // Validate that at least one date is selected
                if (dto.RequestedDates == null || dto.RequestedDates.Count == 0)
                {
                    return BadRequest("At least one date must be selected");
                }

                // Normalize requested dates to just the date part (remove time)
                var requestedDates = dto.RequestedDates.Select(d => d.Date).ToList();

                // Validate that requested dates are within available time slots
                var availableDates = post.AvailableTimeSlots.Select(d => d.Date).ToHashSet();

                foreach (var requestedDate in requestedDates)
                {
                    if (!availableDates.Contains(requestedDate))
                    {
                        return BadRequest($"Date {requestedDate:yyyy-MM-dd} is not available for booking");
                    }
                }

                // Check for overlapping requests that are approved or pending
                var existingRequests = await _context.Requests
                    .Where(r =>
                        r.PostId == dto.PostId &&
                        (r.Status == RequestStatus.Approved || r.Status == RequestStatus.Pending))
                    .ToListAsync();

                foreach (var existingRequest in existingRequests)
                {
                    var existingDates = existingRequest.RequestedDates.Select(d => d.Date).ToHashSet();
                    var overlap = requestedDates.Any(d => existingDates.Contains(d));

                    if (overlap)
                    {
                        return BadRequest("One or more selected dates overlap with an existing booking");
                    }
                }

                var request = new Request
                {
                    UserId = dto.UserId,
                    PostId = dto.PostId,
                    RequestedDates = requestedDates,
                    Status = RequestStatus.Pending
                };

                _context.Requests.Add(request);

                // Remove booked dates from post's available time slots
                var requestedDatesSet = requestedDates.ToHashSet();
                post.AvailableTimeSlots = post.AvailableTimeSlots
                    .Where(slot => !requestedDatesSet.Contains(slot.Date))
                    .ToList();

                // If no more available slots, mark post as unavailable
                if (post.AvailableTimeSlots.Count == 0)
                {
                    post.Available = false;
                    _logger.LogInformation("Post {PostId} marked as unavailable - no more available time slots", post.PostId);
                }

                await _context.SaveChangesAsync();

                // Get client info for notification
                var client = await _context.Users.FindAsync(dto.UserId);
                var car = await _context.CarItems.FindAsync(post.CarId);

                // Create notification for the owner
                var notification = new Notification
                {
                    UserId = post.OwnerId,
                    Type = "booking_request",
                    RequestId = request.ReqId,
                    PostId = post.PostId,
                    Title = "New booking request",
                    Message = $"{client?.Name} {client?.Surname} wants to book your {car?.Brand} {car?.Model}",
                    IsRead = false
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Request created successfully with ID: {RequestId}. Removed {Count} dates from post {PostId}",
                    request.ReqId, requestedDates.Count, post.PostId);

                var response = RequestResponseDto.FromRequest(request);
                return CreatedAtAction("CreateRequest", new { id = request.ReqId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating request");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Requests/{id}/accept
        // Owner accepts a booking request
        [HttpPut("{id}/accept")]
        public async Task<ActionResult> AcceptRequest(int id)
        {
            try
            {
                var request = await _context.Requests
                    .FirstOrDefaultAsync(r => r.ReqId == id);

                if (request == null)
                {
                    return NotFound($"Request with ID {id} not found");
                }

                if (request.Status != RequestStatus.Pending)
                {
                    return BadRequest($"Request is already {request.Status}");
                }

                // Update request status
                request.Status = RequestStatus.Approved;

                // Get post and car info for notification
                var post = await _context.PostItems.FindAsync(request.PostId);
                if (post == null)
                {
                    return BadRequest("Associated post not found");
                }

                var owner = await _context.Users.FindAsync(post.OwnerId);
                var car = await _context.CarItems.FindAsync(post.CarId);

                // Create notification for the client
                var notification = new Notification
                {
                    UserId = request.UserId,
                    Type = "booking_accepted",
                    RequestId = request.ReqId,
                    PostId = request.PostId,
                    Title = "Booking Accepted!",
                    Message = $"{owner?.Name} {owner?.Surname} accepted your booking request for {car?.Brand} {car?.Model}",
                    IsRead = false
                };

                _context.Notifications.Add(notification);

                // Delete ALL notifications for this request (owner's booking_request notifications)
                var notificationsToDelete = await _context.Notifications
                    .Where(n => n.RequestId == id && n.Type == "booking_request")
                    .ToListAsync();

                if (notificationsToDelete.Any())
                {
                    _context.Notifications.RemoveRange(notificationsToDelete);
                    _logger.LogInformation("Removed {Count} notification(s) for request {RequestId}", notificationsToDelete.Count, id);
                }
                else
                {
                    _logger.LogWarning("No booking_request notifications found for request {RequestId}", id);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Request {RequestId} accepted by owner {OwnerId}", id, post.OwnerId);

                return Ok(new { message = "Request accepted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting request {RequestId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Requests/{id}/decline
        // Owner declines a booking request
        [HttpPut("{id}/decline")]
        public async Task<ActionResult> DeclineRequest(int id)
        {
            try
            {
                var request = await _context.Requests
                    .FirstOrDefaultAsync(r => r.ReqId == id);

                if (request == null)
                {
                    return NotFound($"Request with ID {id} not found");
                }

                if (request.Status != RequestStatus.Pending)
                {
                    return BadRequest($"Request is already {request.Status}");
                }

                // Update request status
                request.Status = RequestStatus.Rejected;

                // Get post and car info
                var post = await _context.PostItems.FindAsync(request.PostId);
                if (post == null)
                {
                    return BadRequest("Associated post not found");
                }

                var owner = await _context.Users.FindAsync(post.OwnerId);
                var car = await _context.CarItems.FindAsync(post.CarId);

                // Return the requested dates back to available time slots
                var requestedDatesSet = request.RequestedDates.Select(d => d.Date).ToHashSet();
                var currentAvailableDates = post.AvailableTimeSlots.ToList();

                foreach (var date in request.RequestedDates)
                {
                    if (!currentAvailableDates.Any(d => d.Date == date.Date))
                    {
                        currentAvailableDates.Add(date);
                    }
                }

                post.AvailableTimeSlots = currentAvailableDates.OrderBy(d => d).ToList();
                post.Available = true; // Mark post as available again

                // Create notification for the client
                var notification = new Notification
                {
                    UserId = request.UserId,
                    Type = "booking_rejected",
                    RequestId = request.ReqId,
                    PostId = request.PostId,
                    Title = "Booking Declined",
                    Message = $"{owner?.Name} {owner?.Surname} declined your booking request for {car?.Brand} {car?.Model}",
                    IsRead = false
                };

                _context.Notifications.Add(notification);

                // Delete ALL notifications for this request (owner's booking_request notifications)
                var notificationsToDelete = await _context.Notifications
                    .Where(n => n.RequestId == id && n.Type == "booking_request")
                    .ToListAsync();

                if (notificationsToDelete.Any())
                {
                    _context.Notifications.RemoveRange(notificationsToDelete);
                    _logger.LogInformation("Removed {Count} notification(s) for request {RequestId}", notificationsToDelete.Count, id);
                }
                else
                {
                    _logger.LogWarning("No booking_request notifications found for request {RequestId}", id);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Request {RequestId} declined by owner {OwnerId}. Dates returned to available slots", id, post.OwnerId);

                return Ok(new { message = "Request declined successfully. Dates returned to available slots" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error declining request {RequestId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
