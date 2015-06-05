/// <references path="../all.ts" />

module WebSocketsDemo {

    // combination of:
    // https://developer.mozilla.org/en-US/docs/Web/API/CloseEvent
    // and protocol:
    // https://tools.ietf.org/pdf/rfc6455.pdf#page=45
     
    enum CloseCode {

        // Normal closure; the connection successfully completed whatever purpose
        // for which it was created.
        Normal = 1000,

        // The endpoint is going away, either because of a server failure or because
        // the browser is navigating away from the page that opened the connection.
        GoingAway = 1001,

        // The endpoint is terminating the connection due to a protocol error.
        ProtocolError = 1002,

        // The connection is being terminated because the endpoint received data
        // of a type it cannot accept (for example, a text- only endpoint received
        // binary data).
        UnsupportedData = 1003,

        // Reserved. A meaning might be defined in the future.
        Reserved1004 = 1004, 

        // Reserved. Indicates that no status code was provided even though one was expected.
        NoStatus = 1005,

        // Reserved. Used to indicate that a connection was closed abnormally (that is,
        // with no close frame being sent) when a status code is expected.
        AbnormalClosure = 1006,

        // The endpoint is terminating the connection because a message was received
        // that contained inconsistent data (e.g., non-UTF-8 data within a text message).
        InvalidFramePayloadData = 1007,

        // The endpoint is terminating the connection because it received a message that
        // violates its policy. 
        // This is a generic status code, used when codes 1003 and 1009 are not suitable.
        PolicyViolation = 1008,

        // The endpoint is terminating the connection because a data frame was received
        // that is too large.
        MessageTooBig = 1009,

        // The client is terminating the connection because it expected the server to
        // negotiate one or more extension, but the server didn't.
        MandatoryExtension = 1010,

        // The server is terminating the connection because it encountered an unexpected
        // condition that prevented it from fulfilling the request.
        InternalServerError = 1011,

        // Reserved
        Reserved1012 = 1012,
        Reserved1013 = 1013,
        Reserved1014 = 1014,

        // Reserved. Indicates that the connection was closed due to a failure to perform
        // a TLS handshake (e.g., the server certificate can't be verified).
        TlsHandshake = 1015,

        // 1016–1999 	  	Reserved for future use by the WebSocket standard.
        //
        // 2000–2999 	  	Reserved for use by WebSocket extensions.
        //
        // 3000–3999 	  	Available for use by libraries and frameworks.
        //                  May not be used by applications.
        //
        // 4000–4999 	  	Available for use by applications.
    }


    export class SubscriptionController {

        private ws: WebSocket;
        private subscribed: { [type: string]: { (message: Object): void; }[] };
        IsOpen: boolean;
        OnOpen: { (): void; };
        OnClose: { (): void; };
        OnMessage: { (message: string): void; };

        public static $inject = [
            '$scope',
            '$location'
        ];

        constructor(
            private $scope: ISubscriptionScope,
            private $location: ng.ILocationService
            ) {
            $scope.vm = this;
        }

        Open() {
            this.ws = new WebSocket("ws://" + this.$location.host + ":" + this.$location.port + "/session");
            this.ws.onclose = this.wsClose;
            this.ws.onmessage = this.wsMessage;
            this.ws.onopen = this.wsOpen;
            this.ws.binaryType = 'arraybuffer';
        }

        Close() {
            this.ws.close(CloseCode.Normal, "normal close");
        }

        Publish(topic: string, message: Object) {
            var msg = new PubSub.Messages.TopicUpdate();
            msg.Topic = topic;
            msg.Content = JSON.stringify(message);
            this.ws.send(msg);
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

        private wsOpen() {
            this.IsOpen = true;
            if (this.OnOpen)
                this.OnOpen();
        }

        private wsClose() {
            this.IsOpen = false;
            if (this.OnClose)
                this.OnClose();
        }

        private wsMessage(ev: MessageEvent) {
            if (!ev.data)
                return;

            // get message
            var msg = JSON.parse(ev.data);
            if (msg == null)
                return;

            // raise event
            if (this.OnMessage)
                this.OnMessage(msg);

            // determine type
            var type = null;
            if (msg.Type != null)
                type = msg.Type;
            else if (msg.$type != null)
                type = msg.$type;
            if (type == null)
                return;

            // call subscribers
            var list = this.subscribed[type];
            if (list == null)
                return;

            list.forEach((f) => f(msg));
        }
    }
}