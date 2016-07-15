using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.AspNet.Mvc;
using Messages;

namespace PubSub
{
    public delegate void PublishServerHandler(IPublisherServer session, Message m);
    public delegate void PublishClientHandler(IPublisherClient session, Message m);

    public interface IPublisher
    {
        Task<IPublisherClient> ConnectAsync(string endpoint, CancellationToken token);
    }

    public interface IPublisherSession
    {
        Task PublishAsync(string topic, string content);
        Task SubscribeAsync(string topic);
        Task UnsubscribeAsync(string topic);
        Task<IActionResult> WaitAsync(CancellationToken token);
        Task<IActionResult> WaitAsync();
    }

    public interface IPublisherServer : IPublisherSession
    {
        event PublishServerHandler OnPublish;
    }

    public interface IPublisherClient : IPublisherSession
    {
        event PublishClientHandler OnPublish;
    }
}