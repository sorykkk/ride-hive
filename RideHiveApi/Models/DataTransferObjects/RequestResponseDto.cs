using RideHiveApi.Models.Enums;

namespace RideHiveApi.Models.DataTransferObjects
{
    public class RequestResponseDto
    {
        public int ReqId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int PostId { get; set; }
        public List<string> RequestedDates { get; set; } = new List<string>();
        public RequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public static RequestResponseDto FromRequest(Request request)
        {
            return new RequestResponseDto
            {
                ReqId = request.ReqId,
                UserId = request.UserId,
                PostId = request.PostId,
                RequestedDates = request.RequestedDates
                    .Select(d => d.ToString("o")) // ISO 8601 format
                    .ToList(),
                Status = request.Status,
                CreatedAt = request.CreatedAt
            };
        }
    }
}
