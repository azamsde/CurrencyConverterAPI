namespace CurrencyConverterAPI.Services.Interfaces
{
    public interface ICurrencyService
    {
        Task<object> GetLatestRates(string baseCurrency);
        Task<object> ConvertCurrency(string from, string to, decimal amount);
        Task<object> GetHistoricalRates(string baseCurrency, string startDate, string endDate, int page, int pageSize);
    }
}
