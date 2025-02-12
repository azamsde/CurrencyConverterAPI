using CurrencyConverterAPI.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace CurrencyConverterAPI.Tests
{
    public class FakeCurrencyService : ICurrencyService
    {
        public async Task<object> GetLatestRates(string baseCurrency)
        {
            await Task.Delay(50); // Simulate async call
            return new { rates = new { EUR = 0.85, GBP = 0.75 } };
        }

        public async Task<object> ConvertCurrency(string from, string to, decimal amount)
        {
            await Task.Delay(50);
            return new { convertedAmount = amount * 0.85m };
        }

        public async Task<object> GetHistoricalRates(string baseCurrency, string startDate, string endDate, int page = 1, int pageSize = 10)
        {
            await Task.Delay(50);
            return new { rates = new { date= "2023-01-01" ,  USD = 1.05  } };
        }
    }
}
