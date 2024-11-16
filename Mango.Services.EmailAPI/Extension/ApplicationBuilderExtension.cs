using Mango.Services.EmailAPI.Messaging;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace Mango.Services.EmailAPI.Extension
{
    //class must be static cause it's an extension
    public static class ApplicationBuilderExtension
    {
        private static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }   
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            //implement and notify the application lifetime
            var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            //call the services Defined in AzureServiceBusConsumer Messaging
            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStarted.Register(OnStop);

            return app;
        }

        private static void OnStart()
        {
            ServiceBusConsumer.Start();
        }

        private static void OnStop()
        {
            ServiceBusConsumer.Stop();
        }
    }
}
