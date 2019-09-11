using System.Net.Http;
using GeoCoding.Interfaces;

namespace GeoCoding.Services
{
    public static class GeoServiceFactory
    {
        public static IGeoService CreateGoogleService(HttpClient httpClient, string apiKey)
        {
            return new GoogleService(apiKey, httpClient);
        }

        public static IGeoService CreateBingService(HttpClient httpClient, string apiKey)
        {
            return new BingService(apiKey, httpClient);
        }
    }
}
