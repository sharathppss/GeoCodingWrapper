using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GeoCoding.Utils.Http
{
    public class GeoHandler : DelegatingHandler
    {
        private readonly int retryCount;

        public GeoHandler(int retryCount)
        : this(new HttpClientHandler(), retryCount)
        { }

        public GeoHandler(HttpMessageHandler innerHandler, int retryCount)
            : base(innerHandler)
        {
            this.retryCount = retryCount;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            Trace.TraceInformation($"GeoCoding Web Request: Sending {request.Method} {request.RequestUri.ToString()}");

            for (int i = 0; i <= retryCount; i++)
            {
                response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }

            Trace.TraceInformation($"GeoCoding Web Request: Successful {response.Content}");

            return response;
        }
    }

    public static class UriHelper
    {
        public static Uri GenerateUri(string baseUri, IDictionary<string, string> querystringParams)
        {
            var uriBuilder = new UriBuilder(baseUri);
            var parameters = HttpUtility.ParseQueryString(string.Empty);

            foreach(var element in querystringParams)
            {
                parameters[element.Key] = element.Value;
            }

            uriBuilder.Query = parameters.ToString();
            return uriBuilder.Uri;
        }
    }
}
