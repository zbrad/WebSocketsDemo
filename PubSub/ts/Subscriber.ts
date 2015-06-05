// <reference path="../Sessions/lib/Sessions.d.ts" />

module PubSub {
    export class Subscriber {
        private session: Sessions.ISession;
        private subscribed: { [type: string]: { (message: Object): void; }[] };

        constructor(session: Sessions.ISession) {
            this.session = session;
        }
        
        Publish(topic: string, message: Sessions.IMessage) {
            var msg = new TopicUpdate();
            msg.Topic = topic;
            msg.Content = JSON.stringify(message);
            this.session.Send(msg);
        }

        Subscribe(topic: string, func: (message: Object) => void) {
            var list = this.subscribed[topic];
            if (list == null)
                list = this.subscribed[topic] = new Array<(message: Object) => void>();
            list.push(func);

            var sub = new PubSub.Messages.Subscribe();
            sub.Topic = topic;
            this.ws.send(JSON.stringify(sub));
        }

        Unsubscribe(topic: string, func: (message: Object) => void) {
            var list = this.subscribed[topic];
            var index = list.indexOf(func);
            if (index >= 0)
                list.splice(index, 1);

            var unsub = new PubSub.Messages.Unsubscribe();
            unsub.Topic = topic;
            this.ws.send(JSON.stringify(unsub));
        }

        private pubMessage(message: Sessions.Message) {

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

            list.forEach((f) => f(message));
        }
    }
}