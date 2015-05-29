using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.ConfigurationModel;

namespace Sessions
{
    public static class Extensions
    {
        public static void AddSessions(this IServiceCollection services)
        {
            services.AddSingleton<ISessionFactory, SessionFactory>();
            services.AddTransient<IServerSession, ServerSession>();
            services.AddTransient<IClientSession, ClientSession>();
        }
    }
}
