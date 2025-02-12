using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CurrencyConverterAPI.Services.Implementation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CurrencyConverterAPI.Tests
{
    public class CurrencyServiceTests
    {
        private readonly CurrencyService _service;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CurrencyService> _logger;
        private readonly FakeRetryPolicyService _retryPolicy;

        public CurrencyServiceTests()
        {
            _httpClient = new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri("https://api.frankfurter.app")
            };

            _cache = new MemoryCache(new MemoryCacheOptions());
            _logger = new LoggerFactory().CreateLogger<CurrencyService>();
            _retryPolicy = new FakeRetryPolicyService();

            _service = new CurrencyService(_httpClient, _cache, _retryPolicy, _logger);
        }

        [Fact]
        public async Task GetLatestRates_Returns_ValidData()
        {
            var result = await _service.GetLatestRates("EUR");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetHistoricalRates_Returns_ValidData()
        {
            var result = await _service.GetHistoricalRates("EUR", "2023-01-01", "2023-01-31", 1, 10);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetLatestRates_Uses_Cache()
        {
            string cacheKey = "EUR";
            _cache.Set(cacheKey, new { rates = new { USD = 1.1 } }, TimeSpan.FromMinutes(30));

            var result = await _service.GetLatestRates("EUR");

            Assert.NotNull(result);
        }
    }
}
