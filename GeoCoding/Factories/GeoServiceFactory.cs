using System.Net.Http;
using GeoCoding.Interfaces;

namespace GeoCoding.Services
{
    public static class GeoServiceFactory
    {
        public static IGeoService CreateGoogleService(string apiKey)
        {
            var httpClient = new HttpClient(new Utils.Http.GeoHandler(3));
            return new GoogleService(apiKey, httpClient);
        }

        public static IGeoService CreateBingService(string apiKey)
        {
            var httpClient = new HttpClient(new Utils.Http.GeoHandler(3));
            return new BingService(apiKey, httpClient);
        }
    }
}
