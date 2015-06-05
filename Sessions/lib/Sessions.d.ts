/// <reference path="../typings/all.d.ts" />
export interface IMessage {
    $type?: string;
    Type: string;
}
export interface IError extends IMessage {
    Message: string;
}
export interface ISessionFactory {
}
export interface ISession {
    Id: string;
    IsClosed: boolean;
    CloseStatus: CloseCode;
    CloseDescription: string;
    Close(): void;
    OnClose: {
        (): void;
    };
    OnMessage: {
        (message: IMessage): void;
    };
    OnError: {
        (error: IError): void;
    };
}
export interface IServerSession extends ISession {
}
export interface IClientSession extends ISession {
    Open(endpoint: string): void;
    OnOpen: {
        (): void;
    };
}
export declare class Message implements IMessage {
    $type: string;
    Type: string;
    constructor();
}
export declare class Error extends Message implements IError {
    Message: string;
}
export declare enum CloseCode {
    Normal = 1000,
    GoingAway = 1001,
    ProtocolError = 1002,
    UnsupportedData = 1003,
    Reserved1004 = 1004,
    NoStatus = 1005,
    AbnormalClosure = 1006,
    InvalidFramePayloadData = 1007,
    PolicyViolation = 1008,
    MessageTooBig = 1009,
    MandatoryExtension = 1010,
    InternalServerError = 1011,
    Reserved1012 = 1012,
    Reserved1013 = 1013,
    Reserved1014 = 1014,
    TlsHandshake = 1015,
}
