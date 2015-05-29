module PubSub.Messages {

    export class Message {
        Type: string;
        constructor() {
            this.Type = typeof this;
        }
    }

    export class Subscribe extends Message {
        Topic: string;
    }

    export class TopicUpdate extends Message {
        Publisher: string;
        Date: Date;
        Topic: string;
        Content: string;
    }

    export class Error extends Message {
        Message: string;
    }

    export class GetSubscriptions {
    }

    export class Subscriptions {
        List: string[];
    }

}