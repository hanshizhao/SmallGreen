using RestSharp;

namespace SmallGreen.Desktop.Settings.Services
{
    public class BaseRequest
    {
        public Method Method { get; set; }
        public string Route { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/json";
        public object? Parameter { get; set; }
    }
}
