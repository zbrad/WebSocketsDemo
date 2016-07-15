var WebDemo;
(function (WebDemo) {
    var Subscriber = (function () {
        function Subscriber(session) {
            this.session = session;
        }
        Subscriber.prototype.Publish = function (topic, message) {
            var msg = new WebDemo.TopicUpdate();
            msg.Topic = topic;
            msg.Content = JSON.stringify(message);
            this.session.Send(msg);
        };
        Subscriber.prototype.Subscribe = function (topic, func) {
            var list = this.subscribed[topic];
            if (list == null)
                list = this.subscribed[topic] = new Array();
            list.push(func);
            var sub = new WebDemo.Subscribe();
            sub.Topic = topic;
            this.session.Send(sub);
        };
        Subscriber.prototype.Unsubscribe = function (topic, func) {
            var list = this.subscribed[topic];
            var index = list.indexOf(func);
            if (index >= 0)
                list.splice(index, 1);
            var unsub = new WebDemo.Unsubscribe();
            unsub.Topic = topic;
            this.session.Send(unsub);
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
    WebDemo.Subscriber = Subscriber;
})(WebDemo || (WebDemo = {}));
