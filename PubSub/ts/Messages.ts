// <reference path="../Sessions/lib/Sessions.d.ts" />
// <reference path="../Sessions/lib/Messages.d.ts" />

module PubSub {

    export class Subscribe extends Message {
        Topic: string;
    }

    export class Unsubscribe extends Sessions.Message {
        Topic: string;
    }

    export class TopicUpdate extends Sessions.Message {
        Publisher: string;
        Date: Date;
        Topic: string;
        Content: string;
    }

    export class GetSubscriptions {
    }

    export class GetTopics {
    }

    export class Subscriptions {
        List: string[];
    }

    export class Topics {
        List: string[];
    }

}