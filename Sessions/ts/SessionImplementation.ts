/// <reference path="../typings/all.d.ts"/>
/// <reference path="../typings/uuid/UUID.d.ts"/>

module Sessions {

    class ServerSession extends Session implements S.IServerSession {

        constructor(factory: S.ISessionFactory, ws: WebSocket) {
            super(factory);
            super.SetSocket(ws);
        }
    }

    class ClientSession extends Session implements S.IClientSession {
        Endpoint: string;
        OnOpen: { (): void; };

        constructor(factory: S.ISessionFactory) {
            super(factory);
        }

        Open(endpoint: string) {
            var ws = new WebSocket(this.Endpoint);
            ws.onopen = this.wsOpen;
            super.SetSocket(ws);
        }

        private wsOpen() {
            this.IsClosed = false;
            if (this.OnOpen)
                this.OnOpen();
        }

    }

    class Session implements S.ISession {
        protected ws: WebSocket;
        protected factory: S.ISessionFactory;

        Id: string;
        Endpoint: string;
        IsClosed: boolean;
        CloseStatus: S.CloseCode;
        CloseDescription: string;

        OnClose: { (): void; };
        OnMessage: { (message: S.IMessage): void; };
        OnError: { (error: S.IError): void; };

        constructor(factory: S.ISessionFactory) {
            this.factory = factory;
            this.Id = UUID.generate();
            this.IsClosed = true;
        }

        SetSocket(ws: WebSocket) {
            this.ws = ws;
            this.ws.onclose = this.wsClose;
            this.ws.onmessage = this.wsMessage;
            this.ws.onerror = this.wsError;
            this.ws.binaryType = 'arraybuffer';
        }

        Close() {
            this.ws.close(S.CloseCode.Normal, "normal close");
        }

        private wsClose() {
            this.IsClosed = true;
            if (this.OnClose)
                this.OnClose();
        }

        private wsMessage(ev: MessageEvent) {
            if (!ev.data)
                return;

            // get message
            var msg = <S.IMessage>JSON.parse(ev.data);
            if (msg == null)
                return;

            if (msg.Type === 'Error' && this.OnError)
                this.OnError(<S.IError> msg);

            // raise event
            if (this.OnMessage)
                this.OnMessage(msg);
        }

        private wsError(ev: Event) {
            if (this.OnError) {
                var error = new S.Error();
                error.Message = ev.type;
                this.OnError(error);
            }
        }
    }
}