using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.WebSockets.Server;

namespace SessionLib
{
    public sealed class Sessions
    {
        RequestDelegate next;
        public Sessions(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context, IServerSession session)
        {
            if (session == null)
                return this.next(context);

            return session.WaitAsync();
        }
    }
}
