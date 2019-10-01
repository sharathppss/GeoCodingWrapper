using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GeoCoding.Services;
using RichardSzalay.MockHttp;
using Xunit;
using UnitTests.Utils;
using GeoCoding.Data;
using GeoCoding.Errors;

namespace GeoCodingUnitTests
{
    public class BingServiceTests
    {
        MockHttpMessageHandler MockHttp = new MockHttpMessageHandler();

        private string BaseUrl = "https://dev.virtualearth.net/REST/v1/Locations";

        IDictionary<string, string> queryParams = new Dictionary<string, string>
        {
            { "q", "sample address" },
            { "key", "SOME_KEY"},
            { "o", "json" }
        };


        [Theory]
        [InlineData(@"../../../BingServiceArtifacts/ValidHighConfidence.json", AccuracyLevel.High)]
        [InlineData(@"../../../BingServiceArtifacts/ValidMediumConfidence.json", AccuracyLevel.Medium)]
        [InlineData(@"../../../BingServiceArtifacts/ValidLowConfidence.json", AccuracyLevel.Low)]

        public async Task HappyFlow(string jsonPath, AccuracyLevel expectedAccuracy)
        {
            var goodResponse = await File.OpenText(jsonPath).ReadToEndAsync();
            MockHttp.When(BaseUrl)
                .Respond("application/json", goodResponse);

            var service = new BingService("SOME_KEY", MockHttp.ToHttpClient());
           
            var coordinates = await service.GetCoordinate("Good address");
            var vals = Generic.ParseCoordinatesFromBingJson(goodResponse);

            Assert.Equal(vals.Item1, coordinates.Latitude);
            Assert.Equal(vals.Item2, coordinates.Longitude);
            Assert.Equal(expectedAccuracy, coordinates.Accuracy);

        }

        [Theory]
        [InlineData(@"../../../BingServiceArtifacts/MissingJsonParams.json")]

        public async Task MissingJsonParams(string jsonPath)
        {
            var missingParamResponse = await File.OpenText(jsonPath).ReadToEndAsync();

            MockHttp.When(BaseUrl)
                .Respond("application/json", missingParamResponse);

            var service = new BingService("SOME_KEY", MockHttp.ToHttpClient());

            await Assert.ThrowsAsync<GCException>(() => service.GetCoordinate("Good address"));

        }

        [Fact]
        public async Task BadResponse()
        {
            MockHttp.When(BaseUrl)
                .Respond(System.Net.HttpStatusCode.BadRequest);

            var service = new BingService("SOME_KEY", MockHttp.ToHttpClient());

            await Assert.ThrowsAsync<GCException>(() => service.GetCoordinate("Good address"));

        }
    }
}
