namespace CurrencyConverterAPI.Services.Interfaces
{
    public interface IRetryPolicyService
    {
        Task<HttpResponseMessage> ExecuteWithRetry(Func<Task<HttpResponseMessage>> action);
    }

}
