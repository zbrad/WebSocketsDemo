module WebDemo {

    class ServerSession extends Session implements IServerSession {

        constructor(factory: ISessionFactory, ws: WebSocket) {
            super(factory);
            super.SetSocket(ws);
        }
    }

    class ClientSession extends Session implements IClientSession {
        Endpoint: string;
        OnOpen: { (): void; };

        constructor(factory: ISessionFactory) {
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

    class Session implements ISession {
        protected ws: WebSocket;
        protected factory: ISessionFactory;

        Id: string;
        Endpoint: string;
        IsClosed: boolean;
        CloseStatus: CloseCode;
        CloseDescription: string;

        OnClose: { (): void; };
        OnMessage: { (message: IMessage): void; };
        OnError: { (error: IError): void; };

        constructor(factory: ISessionFactory) {
            this.factory = factory;
            this.Id = UUID.generate();
            this.IsClosed = true;
        }

        Send(o: Message) {
            this.ws.send(JSON.stringify(o));
        }

        SetSocket(ws: WebSocket) {
            this.ws = ws;
            this.ws.onclose = this.wsClose;
            this.ws.onmessage = this.wsMessage;
            this.ws.onerror = this.wsError;
            this.ws.binaryType = 'arraybuffer';
        }

        Close() {
            this.ws.close(CloseCode.Normal, "normal close");
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
            var msg = <IMessage>JSON.parse(ev.data);
            if (msg == null)
                return;

            if (msg.Type === 'Error' && this.OnError)
                this.OnError(<IError> msg);

            // raise event
            if (this.OnMessage)
                this.OnMessage(msg);
        }

        private wsError(ev: Event) {
            if (this.OnError) {
                var error = new Error();
                error.Message = ev.type;
                this.OnError(error);
            }
        }
    }
}