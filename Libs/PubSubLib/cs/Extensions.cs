using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.ConfigurationModel;
using MessageLib;
using Microsoft.AspNet.Builder;

namespace PubSubLib
{
    public static class Extensions
    {
        public static void AddPublishers(this IServiceCollection services)
        {
            services.AddMessengers();
            services.AddSingleton<IPublisherFactory, PublisherFactory>();
            services.AddTransient<IServerSubscriber, ServerSubscriber>();
        }

        public static void UsePublishers(this IApplicationBuilder app)
        {
            app.UseMiddleware<Publishers>();
        }
    }
}
