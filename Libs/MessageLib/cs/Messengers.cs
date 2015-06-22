using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace MessageLib
{
    public sealed class Messengers
    {
        RequestDelegate next;

        public Messengers(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context, IServerMessenger messenger)
        {
            if (messenger == null)
                return this.next(context);

            return messenger.WaitAsync();
        }
    }
}
