declare module Messages {
    interface IMessage {
        $type?: string;
        Type: string;
    }
    interface IError extends IMessage {
        Message: string;
    }
    class Message implements IMessage {
        $type: string;
        Type: string;
        constructor();
    }
    class Error extends Message implements IError {
        Message: string;
    }
}
