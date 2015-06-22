using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace PubSubLib
{
    public sealed class Publishers
    {
        RequestDelegate next;
        public Publishers(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context, IServerSubscriber subscriber)
        {
            if (subscriber == null)
                return this.next(context);

            return subscriber.WaitAsync();
        }
    }
}
