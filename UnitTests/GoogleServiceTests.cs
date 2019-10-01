using System.IO;
using System.Threading.Tasks;
using GeoCoding.Services;
using RichardSzalay.MockHttp;
using Xunit;
using UnitTests.Utils;
using GeoCoding.Data;
using GeoCoding.Errors;

namespace AddressTranslationTests
{
    public class GoogleServiceTests
    {
        MockHttpMessageHandler MockHttp = new MockHttpMessageHandler();

        private string BaseUrl = "https://maps.googleapis.com/maps/api/geocode/json";

        [Theory]
        [InlineData(@"../../../GoogleServiceArtifacts/ValidRoofTopMethod.json", AccuracyLevel.High)]
        [InlineData(@"../../../GoogleServiceArtifacts/ValidInterpolationMethod.json", AccuracyLevel.Medium)]
        [InlineData(@"../../../GoogleServiceArtifacts/ValidGeometricMethod.json", AccuracyLevel.Low)]
        [InlineData(@"../../../GoogleServiceArtifacts/ValidApproximateMethod.json", AccuracyLevel.Lowest)]

        public async Task HappyFlow(string jsonPath, AccuracyLevel expectedAccuracy)
        {
            var goodResponse = await File.OpenText(jsonPath).ReadToEndAsync();

            MockHttp.When(BaseUrl)
                .Respond("application/json", goodResponse);

            var service = new GoogleService("SOME_KEY", MockHttp.ToHttpClient());

            var coordinates = await service.GetCoordinate("Good address");
            var vals = Generic.ParseCoordinatesFromGoogleJson(goodResponse);

            Assert.Equal(vals.Item1, coordinates.Latitude);
            Assert.Equal(vals.Item2, coordinates.Longitude);
            Assert.Equal(expectedAccuracy, coordinates.Accuracy);

        }

        [Theory]
        [InlineData(@"../../../GoogleServiceArtifacts/MissingJsonParams.json")]

        public async Task MissingJsonParams(string jsonPath)
        {
            var missingParamResponse = await File.OpenText(jsonPath).ReadToEndAsync();

            MockHttp.When(BaseUrl)
                .Respond("application/json", missingParamResponse);

            var service = new GoogleService("SOME_KEY", MockHttp.ToHttpClient());

            await Assert.ThrowsAsync<GCException>(() => service.GetCoordinate("Good address"));

        }

        [Theory]
        [InlineData(@"../../../GoogleServiceArtifacts/InvalidStatus.json")]

        public async Task InvalidStatus(string jsonPath)
        {
            var invalidStatusResponse = await File.OpenText(jsonPath).ReadToEndAsync();

            MockHttp.When(BaseUrl)
                .Respond("application/json", invalidStatusResponse);

            var service = new GoogleService("SOME_KEY", MockHttp.ToHttpClient());

            await Assert.ThrowsAsync<GCException>(() => service.GetCoordinate("Good address"));

        }

        [Fact]
        public async Task BadResponse()
        {
            MockHttp.When(BaseUrl)
                .Respond(System.Net.HttpStatusCode.BadRequest);

            var service = new GoogleService("SOME_KEY", MockHttp.ToHttpClient());

            await Assert.ThrowsAsync<GCException>(() => service.GetCoordinate("Good address"));

        }
    }
}
