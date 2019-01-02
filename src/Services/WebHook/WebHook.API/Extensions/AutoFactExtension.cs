using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebHook.API.Extensions
{
    public static class AutoFactExtension
    {
        public static IServiceProvider AddContainer(this IServiceCollection services)
        {
            var container = new ContainerBuilder();

            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }
    }
}
