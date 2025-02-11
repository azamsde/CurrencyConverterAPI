using CurrencyConverterAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace CurrencyConverterAPI.Services.Implementation
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IRetryPolicyService _retryPolicyService;
        private const string BaseUrl = "https://api.frankfurter.app";
        private static readonly HashSet<string> _excludedCurrencies = new HashSet<string> { "TRY", "PLN", "THB", "MXN" };

        public CurrencyService(HttpClient httpClient, IMemoryCache cache, IRetryPolicyService retryPolicyService)
        {
            _httpClient = httpClient;
            _cache = cache;
            _retryPolicyService = retryPolicyService;
        }

        public async Task<object> GetLatestRates(string baseCurrency)
        {
            if (_cache.TryGetValue(baseCurrency, out object cachedResponse))
            {
                return cachedResponse;
            }

            string url = $"{BaseUrl}/latest?base={baseCurrency}";
            var response = await _retryPolicyService.ExecuteWithRetry(() => _httpClient.GetAsync(url));

            var responseData = await response.Content.ReadAsStringAsync();
            _cache.Set(baseCurrency, responseData, TimeSpan.FromMinutes(30));
            return JsonSerializer.Deserialize<object>(responseData);
        }

        public async Task<object> ConvertCurrency(string from, string to, decimal amount)
        {
            if (_excludedCurrencies.Contains(from) || _excludedCurrencies.Contains(to))
            {
                throw new ArgumentException("Conversion involving restricted currencies is not allowed.");
            }

            string url = $"{BaseUrl}/latest?from={from}&to={to}&amount={amount}";
            var response = await _retryPolicyService.ExecuteWithRetry(() => _httpClient.GetAsync(url));

            var responseData = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(responseData);
        }

        public async Task<object> GetHistoricalRates(string baseCurrency, string startDate, string endDate, int page, int pageSize)
        {
            string cacheKey = $"history_{baseCurrency}_{startDate}_{endDate}_{page}_{pageSize}";
            if (_cache.TryGetValue(cacheKey, out object cachedResponse))
            {
                return cachedResponse;
            }

            string url = $"{BaseUrl}/{startDate}..{endDate}?base={baseCurrency}";
            var response = await _retryPolicyService.ExecuteWithRetry(() => _httpClient.GetAsync(url));

            var responseData = await response.Content.ReadAsStringAsync();
            _cache.Set(cacheKey, responseData, TimeSpan.FromMinutes(30));
            return JsonSerializer.Deserialize<object>(responseData);
        }
    }

}
