using System.Threading.Tasks;
using GeoCoding.Data;

namespace GeoCoding.Interfaces
{
    public interface IGeoService
    {
        /// <summary>
        /// Get geo coordinate for input address string
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<GeoCoordinate> GetCoordinate(string address);

        /// <summary>
        /// Get valid address string for Geocoordinate input.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        Task<string> GetAddress(GeoCoordinate coordinate);
    }
}
