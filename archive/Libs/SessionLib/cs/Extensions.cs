using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.WebSockets.Server;
using Microsoft.AspNet.Http;


namespace SessionLib
{
    public static class Extensions
    {
        public static void AddSessions(this IServiceCollection services)
        {
            services.AddSingleton<ISessionFactory, SessionFactory>();
            services.AddTransient<IServerSession, ServerSession>();
        }

        public static void UseSessions(this IApplicationBuilder app, string path)
        {
            app.UseWebSockets(new WebSocketOptions { ReplaceFeature = true });
            app.Map(path, wsapp => wsapp.UseMiddleware<Sessions>());
        }
    }
}
