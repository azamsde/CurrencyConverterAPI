using CurrencyConverterAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Polly.CircuitBreaker;
using Polly;
using System.Text.Json;

namespace CurrencyConverterAPI.Services.Implementation
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IRetryPolicyService _retryPolicyService;
        private readonly ILogger<CurrencyService> _logger;
        private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
        private const string BaseUrl = "https://api.frankfurter.app";
        private static readonly HashSet<string> _excludedCurrencies = new HashSet<string> { "TRY", "PLN", "THB", "MXN" };

        public CurrencyService(HttpClient httpClient, IMemoryCache cache, IRetryPolicyService retryPolicyService, ILogger<CurrencyService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _retryPolicyService = retryPolicyService;
            _logger = logger;

            _circuitBreakerPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30),
                    onBreak: (result, span) => _logger.LogWarning("Circuit breaker opened for {Duration} due to {Reason}", span, result.Result.ReasonPhrase),
                    onReset: () => _logger.LogInformation("Circuit breaker reset."),
                    onHalfOpen: () => _logger.LogInformation("Circuit breaker is half-open, testing requests."));
        }

        public async Task<object> GetLatestRates(string baseCurrency)
        {
            _logger.LogInformation("Fetching latest rates for {BaseCurrency}", baseCurrency);
            if (_cache.TryGetValue(baseCurrency, out object cachedResponse))
            {
                return cachedResponse;
            }

            string url = $"{BaseUrl}/latest?base={baseCurrency}";
            var response = await _circuitBreakerPolicy.ExecuteAsync(() => _httpClient.GetAsync(url));
            var responseData = await response.Content.ReadAsStringAsync();
            _cache.Set(baseCurrency, responseData, TimeSpan.FromMinutes(30));
            return JsonSerializer.Deserialize<object>(responseData);
        }

        public async Task<object> GetHistoricalRates(string baseCurrency, string startDate, string endDate, int page, int pageSize)
        {
            _logger.LogInformation("Fetching historical exchange rates for {BaseCurrency} from {StartDate} to {EndDate}", baseCurrency, startDate, endDate);
            string url = $"{BaseUrl}/{startDate}..{endDate}?base={baseCurrency}";
            var response = await _circuitBreakerPolicy.ExecuteAsync(() => _httpClient.GetAsync(url));
            var responseData = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(responseData);
        }

        public async Task<object> ConvertCurrency(string from, string to, decimal amount)
        {
            _logger.LogInformation("Converting {Amount} {FromCurrency} to {ToCurrency}", amount, from, to);
            if (_excludedCurrencies.Contains(from) || _excludedCurrencies.Contains(to))
            {
                throw new ArgumentException("Conversion involving restricted currencies is not allowed.");
            }

            string url = $"{BaseUrl}/latest?from={from}&to={to}&amount={amount}";
            var response = await _retryPolicyService.ExecuteWithRetry(() => _httpClient.GetAsync(url));

            var responseData = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(responseData);
        }
    }
}
