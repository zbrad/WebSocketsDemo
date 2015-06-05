/// <references path="../all.ts" />
var WebSocketsDemo;
(function (WebSocketsDemo) {
    // combination of:
    // https://developer.mozilla.org/en-US/docs/Web/API/CloseEvent
    // and protocol:
    // https://tools.ietf.org/pdf/rfc6455.pdf#page=45
    var CloseCode;
    (function (CloseCode) {
        // Normal closure; the connection successfully completed whatever purpose
        // for which it was created.
        CloseCode[CloseCode["Normal"] = 1000] = "Normal";
        // The endpoint is going away, either because of a server failure or because
        // the browser is navigating away from the page that opened the connection.
        CloseCode[CloseCode["GoingAway"] = 1001] = "GoingAway";
        // The endpoint is terminating the connection due to a protocol error.
        CloseCode[CloseCode["ProtocolError"] = 1002] = "ProtocolError";
        // The connection is being terminated because the endpoint received data
        // of a type it cannot accept (for example, a text- only endpoint received
        // binary data).
        CloseCode[CloseCode["UnsupportedData"] = 1003] = "UnsupportedData";
        // Reserved. A meaning might be defined in the future.
        CloseCode[CloseCode["Reserved1004"] = 1004] = "Reserved1004";
        // Reserved. Indicates that no status code was provided even though one was expected.
        CloseCode[CloseCode["NoStatus"] = 1005] = "NoStatus";
        // Reserved. Used to indicate that a connection was closed abnormally (that is,
        // with no close frame being sent) when a status code is expected.
        CloseCode[CloseCode["AbnormalClosure"] = 1006] = "AbnormalClosure";
        // The endpoint is terminating the connection because a message was received
        // that contained inconsistent data (e.g., non-UTF-8 data within a text message).
        CloseCode[CloseCode["InvalidFramePayloadData"] = 1007] = "InvalidFramePayloadData";
        // The endpoint is terminating the connection because it received a message that
        // violates its policy. 
        // This is a generic status code, used when codes 1003 and 1009 are not suitable.
        CloseCode[CloseCode["PolicyViolation"] = 1008] = "PolicyViolation";
        // The endpoint is terminating the connection because a data frame was received
        // that is too large.
        CloseCode[CloseCode["MessageTooBig"] = 1009] = "MessageTooBig";
        // The client is terminating the connection because it expected the server to
        // negotiate one or more extension, but the server didn't.
        CloseCode[CloseCode["MandatoryExtension"] = 1010] = "MandatoryExtension";
        // The server is terminating the connection because it encountered an unexpected
        // condition that prevented it from fulfilling the request.
        CloseCode[CloseCode["InternalServerError"] = 1011] = "InternalServerError";
        // Reserved
        CloseCode[CloseCode["Reserved1012"] = 1012] = "Reserved1012";
        CloseCode[CloseCode["Reserved1013"] = 1013] = "Reserved1013";
        CloseCode[CloseCode["Reserved1014"] = 1014] = "Reserved1014";
        // Reserved. Indicates that the connection was closed due to a failure to perform
        // a TLS handshake (e.g., the server certificate can't be verified).
        CloseCode[CloseCode["TlsHandshake"] = 1015] = "TlsHandshake";
    })(CloseCode || (CloseCode = {}));
    var SubscriptionController = (function () {
        function SubscriptionController($scope, $location) {
            this.$scope = $scope;
            this.$location = $location;
            $scope.vm = this;
        }
        SubscriptionController.prototype.Open = function () {
            this.ws = new WebSocket("ws://" + this.$location.host + ":" + this.$location.port + "/session");
            this.ws.onclose = this.wsClose;
            this.ws.onmessage = this.wsMessage;
            this.ws.onopen = this.wsOpen;
            this.ws.binaryType = 'arraybuffer';
        };
        SubscriptionController.prototype.Close = function () {
            this.ws.close(CloseCode.Normal, "normal close");
        };
        SubscriptionController.prototype.Publish = function (topic, message) {
            var msg = new PubSub.Messages.TopicUpdate();
            msg.Topic = topic;
            msg.Content = JSON.stringify(message);
            this.ws.send(msg);
        };
        SubscriptionController.prototype.Subscribe = function (topic, func) {
            var list = this.subscribed[topic];
            if (list == null)
                list = this.subscribed[topic] = new Array();
            list.push(func);
            var sub = new PubSub.Messages.Subscribe();
            sub.Topic = topic;
            this.ws.send(JSON.stringify(sub));
        };
        SubscriptionController.prototype.Unsubscribe = function (topic, func) {
            var list = this.subscribed[topic];
            var index = list.indexOf(func);
            if (index >= 0)
                list.splice(index, 1);
            var unsub = new PubSub.Messages.Unsubscribe();
            unsub.Topic = topic;
            this.ws.send(JSON.stringify(unsub));
        };
        SubscriptionController.prototype.wsOpen = function () {
            this.IsOpen = true;
            if (this.OnOpen)
                this.OnOpen();
        };
        SubscriptionController.prototype.wsClose = function () {
            this.IsOpen = false;
            if (this.OnClose)
                this.OnClose();
        };
        SubscriptionController.prototype.wsMessage = function (ev) {
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
            list.forEach(function (f) { return f(msg); });
        };
        SubscriptionController.$inject = [
            '$scope',
            '$location'
        ];
        return SubscriptionController;
    })();
    WebSocketsDemo.SubscriptionController = SubscriptionController;
})(WebSocketsDemo || (WebSocketsDemo = {}));
