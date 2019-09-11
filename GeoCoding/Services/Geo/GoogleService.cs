using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GeoCoding.Data;
using GeoCoding.Errors;
using GeoCoding.Interfaces;
using GeoCoding.Utils.Http;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;

namespace GeoCoding.Services
{
    public class GoogleService: IGeoService
    {
        private static string BaseUri = "https://maps.googleapis.com/maps/api/geocode/json";

        private readonly string APIKey;
        private readonly HttpClient HttpClient;

        internal GoogleService(string apiKey, HttpClient httpClient)
        {
            APIKey = apiKey;
            HttpClient = httpClient;
        }

        public Task<string> GetAddress(GeoCoordinate coordinate)
        {

            throw new NotImplementedException();
        }

        public async Task<GeoCoordinate> GetCoordinate(string address)
        {
            IDictionary<string, string> queryParams = new Dictionary<string, string>
            {
                { "address", address },
                { "key", APIKey }
            };

            Uri requestUri = UriHelper.GenerateUri(BaseUri, queryParams);

            var response = await HttpClient.GetAsync(requestUri);
            var result = await TranslateResponse(response);

            return result;
        }

        private async Task<GeoCoordinate> TranslateResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new GCException($"GoogleService: Received invalid HTTP response code {response.StatusCode}", StatusCode.BadHttpResponse);
            }

            string responseString = await response.Content.ReadAsStringAsync();
            Dictionary<string, dynamic> Parsed = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseString);

            if (Parsed["status"] != "OK")
            {
                throw new GCException($"GoogleService: Received invalid status {Parsed["status"]}", StatusCode.ApiError);
            }

            try
            {
                var geoCodePoint = Parsed["results"][0]["geometry"]["location"];
                string method = Parsed["results"][0]["geometry"]["location_type"];

                return new GeoCoordinate
                {
                    Latitude = geoCodePoint["lat"],
                    Longitude = geoCodePoint["lng"],
                    Accuracy = ConvertFromCalculationMethod(method),
                    Service = ServiceType.Google
                };

            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentException || ex is RuntimeBinderException)
            {
                throw new GCException($"GoogleService: Response JSON recieved - {JsonConvert.SerializeObject(responseString, Formatting.Indented)}",
                    StatusCode.MissingJsonParams);
            }
        }

        private AccuracyLevel ConvertFromCalculationMethod(string method)
        {
            switch(method)
            {
                case "ROOFTOP":
                    return AccuracyLevel.High;
                case "RANGE_INTERPOLATED":
                    return AccuracyLevel.Medium;
                case "GEOMETRIC_CENTER":
                    return AccuracyLevel.Low;
                case "APPROXIMATE":
                    return AccuracyLevel.Lowest;
                default:
                    return AccuracyLevel.Unknown;
            }
        }
    }
}
