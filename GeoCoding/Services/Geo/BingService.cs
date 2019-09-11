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
    public class BingService: IGeoService
    {

        private static readonly string BaseUri = "https://dev.virtualearth.net/REST/v1/Locations";

        private readonly string APIKey;
        private readonly HttpClient HttpClient;

        internal BingService(string apiKey, HttpClient httpClient)
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
                { "q", address },
                { "key", APIKey },
                { "o", "json" }
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
                throw new GCException($"BingService: Received invalid response code {response.StatusCode}", StatusCode.BadHttpResponse);
            }

            string responseString = await response.Content.ReadAsStringAsync();
            Dictionary<string, dynamic> Parsed = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseString);

            try
            {
                var geoCodePoint = Parsed["resourceSets"][0]["resources"][0]["geocodePoints"][0];

                string confidence = Parsed["resourceSets"][0]["resources"][0]["confidence"];

                return new GeoCoordinate
                { 
                    Latitude = geoCodePoint["coordinates"][0],
                    Longitude = geoCodePoint["coordinates"][1],
                    Accuracy = ConvertFromConfidence(confidence),
                    Service = ServiceType.Bing
                };
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentException || ex is RuntimeBinderException)
            {
                throw new GCException($"BingService: Response JSON recieved - {JsonConvert.SerializeObject(responseString, Formatting.Indented)}",
                    StatusCode.MissingJsonParams);
            }
        }

        private AccuracyLevel ConvertFromConfidence(string confidence)
        {
            switch (confidence)
            {
                case "High":
                    return AccuracyLevel.High;
                case "Medium":
                    return AccuracyLevel.Medium;
                case "Low":
                    return AccuracyLevel.Low;
                default:
                    return AccuracyLevel.Unknown;
            }
        }
    }
}
