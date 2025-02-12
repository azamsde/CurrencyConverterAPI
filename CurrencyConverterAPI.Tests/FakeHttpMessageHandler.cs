using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyConverterAPI.Tests
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.Delay(50); // Simulate network delay

            if (request.RequestUri.ToString().Contains("latest"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{ \"rates\": { \"USD\": 1.1, \"GBP\": 0.9 } }")
                };
            }

            if (request.RequestUri.ToString().Contains("history"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{ \"rates\": { \"2023-01-01\": { \"USD\": 1.05 } } }")
                };
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}
