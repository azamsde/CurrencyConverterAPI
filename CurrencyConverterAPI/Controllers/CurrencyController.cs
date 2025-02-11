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

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet("latest/{baseCurrency}")]
        public async Task<IActionResult> GetLatestRates(string baseCurrency)
        {
            return Ok(await _currencyService.GetLatestRates(baseCurrency));
        }

        [HttpGet("convert")]
        public async Task<IActionResult> ConvertCurrency([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal amount)
        {
            return Ok(await _currencyService.ConvertCurrency(from, to, amount));
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistoricalRates([FromQuery] string baseCurrency, [FromQuery] string startDate, [FromQuery] string endDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _currencyService.GetHistoricalRates(baseCurrency, startDate, endDate, page, pageSize));
        }
    }
}
