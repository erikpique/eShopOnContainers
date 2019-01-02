namespace WebHook.API.Models
{
    public class WebHookSettings
    {
        public bool AzureServiceBusEnabled { get; set; }

        public string EventBusConnection { get; set; }

        public string EventBusUserName { get; set; }

        public string EventBusPassword { get; set; }

        public int EventBusRetryCount { get; set; } = 5;

        public string SubscriptionClientName { get; set; }
    }
}
