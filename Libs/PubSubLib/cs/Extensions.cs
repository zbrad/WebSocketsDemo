using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.ConfigurationModel;
using MessageLib;

namespace PubSubLib
{
    public static class Extensions
    {
        public static void AddPublisher(this IServiceCollection services)
        {
            services.AddMessages();
            services.AddSingleton<IPublisherFactory, PublisherFactory>();
            services.AddTransient<IServerSubscriber, ServerSubscriber>();
        }
    }
}
