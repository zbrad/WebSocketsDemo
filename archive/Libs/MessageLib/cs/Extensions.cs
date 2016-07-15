using Microsoft.Framework.DependencyInjection;
using SessionLib;
using Microsoft.AspNet.Builder;

namespace MessageLib
{
    public static class Extensions
    {
        public static void AddMessengers(this IServiceCollection services)
        {
            services.AddSessions();
            services.AddSingleton<IMessengerFactory, MessengerFactory>();
            services.AddTransient<IServerMessenger, ServerMessenger>();
        }

        public static void UseMessengers(this IApplicationBuilder app)
        {
            app.UseMiddleware<Messengers>();
        }
    }
}
