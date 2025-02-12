using CurrencyConverterAPI.Services.Implementation;
using CurrencyConverterAPI.Services.Interfaces;
using Microsoft.OpenApi.Models;

namespace CurrencyConverterAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddHttpClient<ICurrencyService, CurrencyService>();
            services.AddMemoryCache();
            services.AddSingleton<IRetryPolicyService, RetryPolicyService>();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Currency Converter API", Version = "v1" });
            });

            return services;
        }
    }
}
