using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using GeoCoding.Data;
using GeoCoding.Errors;
using GeoCoding.Interfaces;
using Newtonsoft.Json;

namespace GeoCoding
{
    public class AddressTranslator
    {
        private IList<IGeoService> GeoServicePriorityList; // sorted in the prefered order of services.

        /// <summary>
        /// AddressTranslator service with support from multiple geoservices.
        /// </summary>
        /// <param name="geoServicePriorityList">List of geoservices to be used by address translator. Sorted in descending order of preference.</param>
        public AddressTranslator(IList<IGeoService> geoServicePriorityList)
        {
            GeoServicePriorityList = geoServicePriorityList;
        }

        /// <summary>
        /// Retrieve first valid coordinate from geoservice list for specified address string.
        /// Only coordinate satisfiying minimum threshold accuracy are considered.
        /// </summary>
        /// <param name="address">Address string</param>
        /// <param name="threshold">Minimum accuracy expected</param>
        /// <returns>null if no result found. </returns>
        public async Task<GeoCoordinate> GetCoordinate(string address, AccuracyLevel threshold)
        {
            var result = await GetCoordinateHelper(address, threshold, true);
            return (result.Count == 0)? null: result[0];
        }

        /// <summary>
        /// Retrieve all valid coordinates from geoservice list for specified address string.
        /// Only coordinates satisfiying minimum threshold accuracy are included.
        /// </summary>
        /// <param name="address">Address string</param>
        /// <param name="threshold">Minimum accuracy expected</param>
        /// <returns>Empty list if no valid entries found.</returns>
        public async Task<IList<GeoCoordinate>> GetAllCoordinates(string address, AccuracyLevel threshold)
        {
            var result = await GetCoordinateHelper(address, threshold, false);
            return result;
        }

        private async Task<IList<GeoCoordinate>> GetCoordinateHelper(string address, AccuracyLevel threshold, bool stopAtFirst)
        {
            var result = new List<GeoCoordinate>();

            foreach (var geoService in GeoServicePriorityList)
            {
                try
                {
                    var coordinate = await geoService.GetCoordinate(address);

                    if (coordinate.Accuracy < threshold)
                    {
                        Trace.TraceWarning($"{geoService.GetType().ToString()} ignored - Threshold not met\nResponse recieved:\n" +
                            $"{JsonConvert.SerializeObject(coordinate, Formatting.Indented)}");
                        continue;
                    }

                    result.Add(coordinate);
                    if (stopAtFirst) return result;
                }
                catch (GCException ex)
                {
                    Trace.TraceError($"{geoService.GetType().ToString()} failed with exception {ex}");
                }
            }

            return result;
        }
    }

}