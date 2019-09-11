using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UnitTests.Utils
{
    public static class Generic
    {
        public static Tuple<double, double> ParseCoordinatesFromBingJson(string json)
        {
            Dictionary<string, dynamic> Parsed = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
            var geoCodePoint = Parsed["resourceSets"][0]["resources"][0]["geocodePoints"][0];

            double latitude = geoCodePoint["coordinates"][0];
            double longitude = geoCodePoint["coordinates"][1];

            return new Tuple<double, double>(latitude, longitude);
        }

        public static Tuple<double, double> ParseCoordinatesFromGoogleJson(string json)
        {
            Dictionary<string, dynamic> Parsed = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

            var geoCodePoint = Parsed["results"][0]["geometry"]["location"];
            
            double latitude = geoCodePoint["lat"];
            double longitude = geoCodePoint["lng"];

            return new Tuple<double, double>(latitude, longitude);
        }
    }
}
