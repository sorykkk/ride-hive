namespace RideHiveApi.Models.DataTransferObjects
{
    public class NotificationResponseDto
    {
        public int NotificationId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int RequestId { get; set; }
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        // Additional info for frontend display
        public string? UserName { get; set; }
        public string? UserAvatar { get; set; }
        public string? CarName { get; set; }
        public List<string> RequestedDates { get; set; } = new List<string>();

        public static NotificationResponseDto FromNotification(
            Notification notification,
            Request? request = null,
            AppUser? relatedUser = null,
            PostItem? post = null,
            CarItem? car = null)
        {
            var dto = new NotificationResponseDto
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                Type = notification.Type,
                RequestId = notification.RequestId,
                PostId = notification.PostId,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };

            // Add related user info (client for owner notifications, owner for client notifications)
            if (relatedUser != null)
            {
                dto.UserName = $"{relatedUser.Name} {relatedUser.Surname}";
                dto.UserAvatar = relatedUser.ProfileImage != null
                    ? $"/api/user/profile/image/{relatedUser.Id}"
                    : null;
            }

            // Add car name
            if (car != null)
            {
                dto.CarName = $"{car.Brand} {car.Model}";
            }

            // Add requested dates
            if (request != null)
            {
                dto.RequestedDates = request.RequestedDates
                    .Select(d => d.ToString("yyyy-MM-dd"))
                    .ToList();
            }

            return dto;
        }
    }
}
