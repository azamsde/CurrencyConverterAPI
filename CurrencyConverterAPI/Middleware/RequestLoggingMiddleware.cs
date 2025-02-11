namespace CurrencyConverterAPI.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("Handling request: {Method} {Url}", context.Request.Method, context.Request.Path);
            await _next(context);
            _logger.LogInformation("Finished handling request.");
        }
    }

}
