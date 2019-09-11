using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GeoCoding;
using GeoCoding.Interfaces;
using GeoCoding.Services;
using Newtonsoft.Json;

namespace TestTool
{
    //TODO: 1. Add command line parameters for services.
    //      2. Read API Keys from static file.
    //      
    class Program
    {
        static async Task Main(string[] args)
        {
            string sampleAddress = "3530 Hughes Avenue Los Angeles";

            string googleKey = "AIzaSyBqxB6A-05TAGxPqfqIPYAwQh0sk4LWE0Y";
            string bingKey = "AgtV5YJafiu4bnsT8skRfjw - DE0wCPqMUdUHMgUKlYp5qqQsgqeAHJcvEw9M4I__";

            var gcClient = new HttpClient(new GeoCoding.Utils.Http.GeoHandler(3));

            var geoServices = new List<IGeoService>();

            geoServices.Add(GeoServiceFactory.CreateBingService(gcClient, bingKey));
            geoServices.Add(GeoServiceFactory.CreateGoogleService(gcClient, googleKey));


            try
            {
                var translator = new AddressTranslator(geoServices);

                var allResult = await translator.GetAllCoordinates(sampleAddress, GeoCoding.Data.AccuracyLevel.Lowest);
                Console.WriteLine($"GetCoordinate(All) - Address: {sampleAddress}: Coordinates:\n{JsonConvert.SerializeObject(allResult, Formatting.Indented)}");

                var singleResult = await translator.GetCoordinate(sampleAddress, GeoCoding.Data.AccuracyLevel.Lowest);
                Console.WriteLine($"GetCoordinate(first) - Address: {sampleAddress}: Coordinates:\n{JsonConvert.SerializeObject(singleResult, Formatting.Indented)}");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        // Not used.
        Dictionary<string, string> ReadKeys(string filePath)
        {
            var result = new Dictionary<string, string>();
            string text = System.IO.File.ReadAllText(@"APIKeys.txt");

            return result;
        }
    }
}
