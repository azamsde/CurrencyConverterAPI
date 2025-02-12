using System.Threading.Tasks;
using CurrencyConverterAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CurrencyConverterAPI.Tests
{
    public class CurrencyControllerTests
    {
        private readonly CurrencyController _controller;
        private readonly FakeCurrencyService _fakeService;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyControllerTests()
        {
            _fakeService = new FakeCurrencyService();
            _logger = new LoggerFactory().CreateLogger<CurrencyController>();
            _controller = new CurrencyController(_fakeService, _logger);
        }

        [Fact]
        public async Task GetLatestRates_Returns_OkResult()
        {
            var result = await _controller.GetLatestRates("USD");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;
            Assert.NotNull(response);
        }

        [Fact]
        public async Task ConvertCurrency_Returns_OkResult()
        {
            var result = await _controller.ConvertCurrency("USD", "EUR", 100);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;
            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetHistoricalRates_Returns_OkResult()
        {
            var result = await _controller.GetHistoricalRates("USD", "2023-01-01", "2023-01-31");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;
            Assert.NotNull(response);
        }
    }
}
