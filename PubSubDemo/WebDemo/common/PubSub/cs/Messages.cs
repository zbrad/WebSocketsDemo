using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages;

namespace PubSub
{
    public sealed class Subscribe : Message
    {
        public string Topic { get; set; }

        public Subscribe() { }

        public Subscribe(string topic) : this()
        {
            this.Topic = topic;
        }
    }

    public sealed class Unsubscribe : Message
    {
        public string Topic { get; set; }

        public Unsubscribe() { }

        public Unsubscribe(string topic) : this()
        {
            this.Topic = topic;
        }
    }

    public sealed class TopicUpdate : Message
    {
        public string Publisher { get; set; }
        public DateTime Date { get; set; }
        public string Topic { get; set; }
        public string Content { get; set; }

        public TopicUpdate() { }

        public TopicUpdate(string topic, string content)
        {
            this.Topic = topic;
            this.Content = content;
        }
    }

    public sealed class Error : Message
    {
        public string Message { get; set; }

        public Error() { }

        public Error(string e) : this()
        {
            this.Message = e;
        }
    }

    public sealed class GetSubscriptions : Message
    {
        public GetSubscriptions() { }
    }

    public sealed class GetTopics : Message
    {
        public GetTopics() { }
    }

    public sealed class Subscriptions : Message
    {
        public List<string> List { get; set; }
        public Subscriptions() { }

        public Subscriptions(List<string> list)
        {
            this.List = list;
        }
    }

    public sealed class Topics : Message
    {
        public List<string> List { get; set; }
        public Topics() { }

        public Topics(List<string> list)
        {
            this.List = list;
        }
    }
}
