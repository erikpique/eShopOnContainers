using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebHook.API.Models;

namespace WebHook.API.ServiceCollectionExtensions
{
    public static class WebHookSettingsExtension
    {
        public static IServiceCollection WebHookSettingsConfigure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<WebHookSettings>(configuration);

            return services;
        }
    }
}
