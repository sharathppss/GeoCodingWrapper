using System.IO;
using System.Threading.Tasks;
using GeoCoding.Services;
using RichardSzalay.MockHttp;
using Xunit;
using UnitTests.Utils;
using GeoCoding.Data;
using GeoCoding.Errors;
using GeoCoding;
using System.Collections.Generic;
using GeoCoding.Interfaces;
using Moq;
using System.Linq;

namespace GeoCodingUnitTests
{
    public class GoogleServiceTests
    {

        [Theory]

        [InlineData(AccuracyLevel.Medium, AccuracyLevel.Medium, AccuracyLevel.Medium, ServiceType.Google)]
        [InlineData(AccuracyLevel.High, AccuracyLevel.Medium, AccuracyLevel.Medium, ServiceType.Google)]
        [InlineData(AccuracyLevel.Low, AccuracyLevel.Medium, AccuracyLevel.Medium, ServiceType.Bing)]
        [InlineData(AccuracyLevel.Medium, AccuracyLevel.High, AccuracyLevel.Medium, ServiceType.Google)]
        [InlineData(AccuracyLevel.Medium, AccuracyLevel.Low, AccuracyLevel.Medium, ServiceType.Google)]

        public async Task BothActiveServicesGooglePrioritizedAccuracyValidChecks(AccuracyLevel googleAccuracy, AccuracyLevel bingAccuracy,
            AccuracyLevel threshold, ServiceType expectedService)
        {
            var geoServices = new List<IGeoService>();

            var googleServiceMock = new Mock<IGeoService>();
            googleServiceMock.Setup(x => x.GetCoordinate("Good address")).ReturnsAsync(new GeoCoordinate { Accuracy = googleAccuracy, Service = ServiceType.Google });

            var bingServiceMock = new Mock<IGeoService>();
            bingServiceMock.Setup(x => x.GetCoordinate("Good address")).ReturnsAsync(new GeoCoordinate { Accuracy = bingAccuracy, Service = ServiceType.Bing });

            geoServices.Add(googleServiceMock.Object);
            geoServices.Add(bingServiceMock.Object);

            var translator = new AddressTranslator(geoServices);

            var coordinates = await translator.GetCoordinate("Good address", threshold);

            Assert.Equal(expectedService, coordinates.Service);

        }

        [Theory]

        [InlineData(AccuracyLevel.Medium, AccuracyLevel.Medium, AccuracyLevel.Medium, ServiceType.Bing)]
        [InlineData(AccuracyLevel.High, AccuracyLevel.Medium, AccuracyLevel.Medium, ServiceType.Bing)]
        [InlineData(AccuracyLevel.Low, AccuracyLevel.Medium, AccuracyLevel.Medium, ServiceType.Google)]
        [InlineData(AccuracyLevel.Medium, AccuracyLevel.High, AccuracyLevel.Medium, ServiceType.Bing)]
        [InlineData(AccuracyLevel.Medium, AccuracyLevel.Low, AccuracyLevel.Medium, ServiceType.Bing)]

        public async Task BothActiveServicesBingPrioritizedAccuracyValidChecks(AccuracyLevel bingAccuracy, AccuracyLevel googleAccuracy,
            AccuracyLevel threshold, ServiceType expectedService)
        {
            var geoServices = new List<IGeoService>();

            var googleServiceMock = new Mock<IGeoService>();
            googleServiceMock.Setup(x => x.GetCoordinate("Good address")).ReturnsAsync(new GeoCoordinate { Accuracy = googleAccuracy, Service = ServiceType.Google });

            var bingServiceMock = new Mock<IGeoService>();
            bingServiceMock.Setup(x => x.GetCoordinate("Good address")).ReturnsAsync(new GeoCoordinate { Accuracy = bingAccuracy, Service = ServiceType.Bing });

            geoServices.Add(bingServiceMock.Object);
            geoServices.Add(googleServiceMock.Object);

            var translator = new AddressTranslator(geoServices);

            var coordinates = await translator.GetCoordinate("Good address", threshold);

            Assert.Equal(expectedService, coordinates.Service);

        }

        [Theory]

        [InlineData(AccuracyLevel.Medium, AccuracyLevel.Medium, AccuracyLevel.High)]
        [InlineData(AccuracyLevel.Low, AccuracyLevel.Medium, AccuracyLevel.High)]
        [InlineData(AccuracyLevel.Medium, AccuracyLevel.Low, AccuracyLevel.High)]

        public async Task BothActiveServicesBingPrioritizedAccuracyInvalidChecks(AccuracyLevel bingAccuracy, AccuracyLevel googleAccuracy,
            AccuracyLevel threshold)
        {
            var geoServices = new List<IGeoService>();

            var googleServiceMock = new Mock<IGeoService>();
            googleServiceMock.Setup(x => x.GetCoordinate("Good address")).ReturnsAsync(new GeoCoordinate { Accuracy = googleAccuracy, Service = ServiceType.Google });

            var bingServiceMock = new Mock<IGeoService>();
            bingServiceMock.Setup(x => x.GetCoordinate("Good address")).ReturnsAsync(new GeoCoordinate { Accuracy = bingAccuracy, Service = ServiceType.Bing });

            geoServices.Add(googleServiceMock.Object);
            geoServices.Add(bingServiceMock.Object);

            var translator = new AddressTranslator(geoServices);

            var coordinates = await translator.GetCoordinate("Good address", threshold);

            Assert.Null(coordinates);
        }

        [Theory]

        [InlineData(AccuracyLevel.High, AccuracyLevel.High, AccuracyLevel.High, 2)]
        [InlineData(AccuracyLevel.Low, AccuracyLevel.High, AccuracyLevel.High, 1)]
        [InlineData(AccuracyLevel.High, AccuracyLevel.Low, AccuracyLevel.High, 1)]
        [InlineData(AccuracyLevel.Medium, AccuracyLevel.Low, AccuracyLevel.High, 0)]

        public async Task BothActiveServicesAllResponsesCountChecks(AccuracyLevel bingAccuracy, AccuracyLevel googleAccuracy,
            AccuracyLevel threshold, int expectedCount)
        {
            var geoServices = new List<IGeoService>();

            var googleServiceMock = new Mock<IGeoService>();
            googleServiceMock.Setup(x => x.GetCoordinate("Good address")).ReturnsAsync(new GeoCoordinate { Accuracy = googleAccuracy, Service = ServiceType.Google });

            var bingServiceMock = new Mock<IGeoService>();
            bingServiceMock.Setup(x => x.GetCoordinate("Good address")).ReturnsAsync(new GeoCoordinate { Accuracy = bingAccuracy, Service = ServiceType.Bing });

            geoServices.Add(googleServiceMock.Object);
            geoServices.Add(bingServiceMock.Object);

            var translator = new AddressTranslator(geoServices);

            var coordinates = await translator.GetAllCoordinates("Good address", threshold);

            // get no of unique(in terms of services in results) coordinates returned.
            var uniqueCount = coordinates.Select((entry) => { return entry.Service; }).Distinct().Count();

            Assert.Equal(expectedCount, uniqueCount);
        }
    }
}
