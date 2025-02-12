using CurrencyConverterAPI.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace CurrencyConverterAPI.Tests
{
    public class FakeRetryPolicyService : IRetryPolicyService
    {
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            return await action(); // No retries for testing
        }

        public async Task<HttpResponseMessage> ExecuteWithRetry(Func<Task<HttpResponseMessage>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                throw new Exception("FakeRetryPolicyService: Operation failed", ex);
            }
        }
    }
}
