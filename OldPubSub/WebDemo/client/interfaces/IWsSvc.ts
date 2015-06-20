module WebDemo {

    export interface IWsSvc {
        Send(m: IMessage);
        OnMessage: { (message: IMessage); };
    }


}