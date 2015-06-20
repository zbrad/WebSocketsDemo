using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.AspNet.Mvc;
using MessageLib;
using System.Collections.Generic;

namespace PubSubLib
{


    public interface IPublisherFactory
    {
        Task<IClientSubscriber> ConnectAsync(string endpoint, CancellationToken token);
    }

    public delegate Task PublicationHandler(ISubscriber subscriber, Message m);

    public interface ISubscriber
    {
        string Id { get; }
        bool IsClosed { get; }
        Task PublishAsync(string topic, string content);
        Task SubscribeAsync(string topic);
        Task UnsubscribeAsync(string topic);
        Task GetSubscriptionsAsync();
        Task GetTopicsAsync();
        Task WaitAsync();
        Task CloseAsync();
    }

    public interface IServerSubscriber : ISubscriber
    {

    }

    public interface IClientSubscriber : ISubscriber
    {
        event PublicationHandler OnPublication;
        string Endpoint { get; }
    }
}