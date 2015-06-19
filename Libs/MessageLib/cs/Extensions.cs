using Microsoft.Framework.DependencyInjection;
using SessionLib;

namespace MessageLib
{
    public static class Extensions
    {
        public static void AddMessages(this IServiceCollection services)
        {
            services.AddSessions();
            services.AddTransient<IServerMessenger, ServerMessenger>();
        }
    }
}
