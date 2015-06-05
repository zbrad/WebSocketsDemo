/// <reference path="../typings/all.d.ts"/>
/// <reference path="../typings/uuid/UUID.d.ts"/>
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Sessions;
(function (Sessions) {
    var ServerSession = (function (_super) {
        __extends(ServerSession, _super);
        function ServerSession(factory, ws) {
            _super.call(this, factory);
            _super.prototype.SetSocket.call(this, ws);
        }
        return ServerSession;
    })(Session);
    var ClientSession = (function (_super) {
        __extends(ClientSession, _super);
        function ClientSession(factory) {
            _super.call(this, factory);
        }
        ClientSession.prototype.Open = function (endpoint) {
            var ws = new WebSocket(this.Endpoint);
            ws.onopen = this.wsOpen;
            _super.prototype.SetSocket.call(this, ws);
        };
        ClientSession.prototype.wsOpen = function () {
            this.IsClosed = false;
            if (this.OnOpen)
                this.OnOpen();
        };
        return ClientSession;
    })(Session);
    var Session = (function () {
        function Session(factory) {
            this.factory = factory;
            this.Id = UUID.generate();
            this.IsClosed = true;
        }
        Session.prototype.SetSocket = function (ws) {
            this.ws = ws;
            this.ws.onclose = this.wsClose;
            this.ws.onmessage = this.wsMessage;
            this.ws.onerror = this.wsError;
            this.ws.binaryType = 'arraybuffer';
        };
        Session.prototype.Close = function () {
            this.ws.close(S.CloseCode.Normal, "normal close");
        };
        Session.prototype.wsClose = function () {
            this.IsClosed = true;
            if (this.OnClose)
                this.OnClose();
        };
        Session.prototype.wsMessage = function (ev) {
            if (!ev.data)
                return;
            // get message
            var msg = JSON.parse(ev.data);
            if (msg == null)
                return;
            if (msg.Type === 'Error' && this.OnError)
                this.OnError(msg);
            // raise event
            if (this.OnMessage)
                this.OnMessage(msg);
        };
        Session.prototype.wsError = function (ev) {
            if (this.OnError) {
                var error = new S.Error();
                error.Message = ev.type;
                this.OnError(error);
            }
        };
        return Session;
    })();
})(Sessions || (Sessions = {}));
