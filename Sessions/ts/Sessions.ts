/// <reference path="../typings/all.d.ts" />

export interface ISessionFactory {

}

export interface ISession {
    Id: string;
    IsClosed: boolean;
    CloseStatus: CloseCode;
    CloseDescription: string;
    Close(): void;

    OnClose: { (): void; };
    OnMessage: { (message: IMessage): void; };
    OnError: { (error: IError): void; };
}

export interface IServerSession extends ISession {

}

export interface IClientSession extends ISession {
    Open(endpoint: string): void;
    OnOpen: { (): void; };
}

// combination of:
// https://developer.mozilla.org/en-US/docs/Web/API/CloseEvent
// and protocol:
// https://tools.ietf.org/pdf/rfc6455.pdf#page=45
     
export enum CloseCode {

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
