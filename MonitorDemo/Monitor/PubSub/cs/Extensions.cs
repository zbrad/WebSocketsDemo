using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.ConfigurationModel;
using Sessions;

namespace PubSub
{
    public static class Extensions
    {
        public static void AddPublisher(this IServiceCollection services)
        {
            services.AddSessions();
            services.AddSingleton<IPublisher, Publisher>();
            services.AddTransient<IPublisherServer, PublisherServer>();
        }
    }
}
