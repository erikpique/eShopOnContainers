using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebHook.API.Models;

namespace WebHook.API.Extensions
{
    public static class SettingsExtension
    {
        public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions()
                .Configure<WebHookSettings>(configuration);

            return services;
        }
    }
}
