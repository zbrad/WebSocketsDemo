module MessageLib {

    export interface IMessage {
        $type?: string;
        Type: string; 
    }

    export interface IError extends IMessage {
        Message: string;
    }

    export class Message implements IMessage {
        $type: string;
        Type: string;
        constructor() {
            this.$type = this.Type = typeof this;
        }
    }

    export class Error extends Message implements IError {
        Message: string;
    }
}
