module WebDemo {

    export interface ISessionFactory {
         
    }

    export interface ISession {
        Id: string;
        IsClosed: boolean;
        CloseStatus: CloseCode;
        CloseDescription: string;
        Close(): void;
        Send(o: Message): void;
        OnClose: { (): void; };
        OnMessage: { (message: IMessage): void; };
        OnError: { (error: IError): void; };
    }

    export interface IServerSession extends ISession {
        // tbd
    }

    export interface IClientSession extends ISession {
        Open(endpoint: string): void;
        OnOpen: { (): void; };
    }
}
