using CurrencyConverterAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(ICurrencyService currencyService, ILogger<CurrencyController> logger)
        {
            _currencyService = currencyService;
            _logger = logger;
        }

        [HttpGet("latest/{baseCurrency}")]
        public async Task<IActionResult> GetLatestRates(string baseCurrency)
        {
            _logger.LogInformation("Fetching latest exchange rates for {BaseCurrency}", baseCurrency);
            return Ok(await _currencyService.GetLatestRates(baseCurrency));
        }

        [HttpGet("convert")]
        public async Task<IActionResult> ConvertCurrency([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal amount)
        {
            _logger.LogInformation("Converting {Amount} from {FromCurrency} to {ToCurrency}", amount, from, to);
            return Ok(await _currencyService.ConvertCurrency(from, to, amount));
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistoricalRates([FromQuery] string baseCurrency, [FromQuery] string startDate, [FromQuery] string endDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Fetching historical rates for {BaseCurrency} from {StartDate} to {EndDate}", baseCurrency, startDate, endDate);
            return Ok(await _currencyService.GetHistoricalRates(baseCurrency, startDate, endDate, page, pageSize));
        }
    }

}
