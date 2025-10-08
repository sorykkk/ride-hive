namespace RideHiveApi.Models
{
    public class TemperatureResponse
    {
        public double Temperature { get; set; }
        public string Adjective { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}