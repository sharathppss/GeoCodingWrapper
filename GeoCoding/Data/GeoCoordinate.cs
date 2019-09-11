
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GeoCoding.Data
{
    public enum AccuracyLevel
    {
        Lowest = 0,
        Low,
        Medium,
        High,

        Unknown = 999 // Not used.
    };

    public enum ServiceType
    {
        Bing = 0,
        Google = 1,
        MapBox = 2,
    }

    public class GeoCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public AccuracyLevel Accuracy { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceType Service { get; set; }
    };
}
