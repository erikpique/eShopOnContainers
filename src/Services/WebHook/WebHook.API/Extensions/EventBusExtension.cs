using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.ServiceBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus;
using Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.EventHandling;
using Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using WebHook.API.Models;

namespace WebHook.API.Extensions
{
    public static class EventBusExtension
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var webHookSettings = configuration.Get<WebHookSettings>();

            var subscriptionClientName = webHookSettings.SubscriptionClientName;

            if (webHookSettings.AzureServiceBusEnabled)
            {
                AddServiceBus(services, webHookSettings, subscriptionClientName);
            }
            else
            {
                AddRabbitMqBus(services, webHookSettings, subscriptionClientName);
            }

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }

        public static IApplicationBuilder UseEventBusSubscribers(this IApplicationBuilder application)
        {
            var eventBus = application.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();

            return application;
        }

        private static void AddServiceBus(IServiceCollection services, WebHookSettings webHookSettings, string subscriptionClientName)
        {
            services.AddSingleton<IServiceBusPersisterConnection>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();
                var serviceBusConnection = new ServiceBusConnectionStringBuilder(webHookSettings.EventBusConnection);

                return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
            })
            .AddSingleton<IEventBus, EventBusServiceBus>(provider =>
            {
                var serviceBusPersisterConnection = provider.GetRequiredService<IServiceBusPersisterConnection>();
                var iLifetimeScope = provider.GetRequiredService<ILifetimeScope>();
                var logger = provider.GetRequiredService<ILogger<EventBusServiceBus>>();
                var eventBusSubcriptionsManager = provider.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusServiceBus(serviceBusPersisterConnection, logger, eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
            });
        }

        private static void AddRabbitMqBus(IServiceCollection services, WebHookSettings webHookSettings, string subscriptionClientName)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = webHookSettings.EventBusConnection
                };

                if (!string.IsNullOrEmpty(webHookSettings.EventBusUserName))
                {
                    factory.UserName = webHookSettings.EventBusUserName;
                }

                if (!string.IsNullOrEmpty(webHookSettings.EventBusPassword))
                {
                    factory.Password = webHookSettings.EventBusPassword;
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, webHookSettings.EventBusRetryCount);
            })
            .AddSingleton<IEventBus, EventBusRabbitMQ>(provider =>
            {
                var rabbitMQPersistentConnection = provider.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = provider.GetRequiredService<ILifetimeScope>();
                var logger = provider.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = provider.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, webHookSettings.EventBusRetryCount);
            });
        }
    }
}
