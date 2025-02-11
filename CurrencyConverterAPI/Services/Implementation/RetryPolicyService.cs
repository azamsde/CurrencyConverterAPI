using CurrencyConverterAPI.Services.Interfaces;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace CurrencyConverterAPI.Services.Implementation
{
    public class RetryPolicyService : IRetryPolicyService
    {
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;

        public RetryPolicyService()
        {
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            _circuitBreakerPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(3, TimeSpan.FromMinutes(1));
        }

        public async Task<HttpResponseMessage> ExecuteWithRetry(Func<Task<HttpResponseMessage>> action)
        {
            return await _retryPolicy.ExecuteAsync(() => _circuitBreakerPolicy.ExecuteAsync(action));
        }
    }
}
