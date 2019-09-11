using System;
using System.Threading.Tasks;
using GeoCoding.Data;
using GeoCoding.Interfaces;

namespace GeoCoding.Services
{
    public class MapBoxService: IGeoService
    {
        internal MapBoxService() { }

        public Task<string> GetAddress(GeoCoordinate coordinate)
        {
            throw new NotImplementedException();
        }

        public Task<GeoCoordinate> GetCoordinate(string address)
        {
            throw new NotImplementedException();
        }
    }
}
 