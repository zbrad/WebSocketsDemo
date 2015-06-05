// <reference path="../Sessions/lib/Sessions.d.ts" />
var PubSub;
(function (PubSub) {
    var Subscriber = (function () {
        function Subscriber(session) {
            this.session = session;
        }
        Subscriber.prototype.Publish = function (topic, message) {
            var msg = new PubSub.TopicUpdate();
            msg.Topic = topic;
            msg.Content = JSON.stringify(message);
            this.session.Send(msg);
        };
        Subscriber.prototype.Subscribe = function (topic, func) {
            var list = this.subscribed[topic];
            if (list == null)
                list = this.subscribed[topic] = new Array();
            list.push(func);
            var sub = new PubSub.Messages.Subscribe();
            sub.Topic = topic;
            this.ws.send(JSON.stringify(sub));
        };
        Subscriber.prototype.Unsubscribe = function (topic, func) {
            var list = this.subscribed[topic];
            var index = list.indexOf(func);
            if (index >= 0)
                list.splice(index, 1);
            var unsub = new PubSub.Messages.Unsubscribe();
            unsub.Topic = topic;
            this.ws.send(JSON.stringify(unsub));
        };
        Subscriber.prototype.pubMessage = function (message) {
            // determine type
            var type = null;
            if (message.Type != null)
                type = message.Type;
            else if (message.$type != null)
                type = message.$type;
            if (type == null)
                return;
            // call subscribers
            var list = this.subscribed[type];
            if (list == null)
                return;
            list.forEach(function (f) { return f(message); });
        };
        return Subscriber;
    })();
    PubSub.Subscriber = Subscriber;
})(PubSub || (PubSub = {}));
