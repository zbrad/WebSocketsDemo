module WebDemo {

    export interface ISubscribe extends IMessage {
        Topic: string;
    }

    export class Subscribe extends Message implements ISubscribe {
        Topic: string;
    }

    export interface IUnsubscribe extends IMessage {
        Topic: string;
    }

    export class Unsubscribe extends Message implements IUnsubscribe {
        Topic: string;
    }

    export interface ITopicUpdate extends IMessage {
        Publisher: string;
        Date: Date;
        Topic: string;
        Content: string;
    }

    export class TopicUpdate extends Message implements ITopicUpdate {
        Publisher: string;
        Date: Date;
        Topic: string;
        Content: string;
    }

    export interface IGetSubscriptions extends IMessage {
    }

    export class GetSubscriptions extends Message implements IGetSubscriptions {
    }

    export interface IGetTopics extends IMessage {
    }

    export class GetTopics extends Message implements IGetTopics  {
    }

    export interface ISubscriptions extends IMessage {
        List: string[];
    }

    export class Subscriptions extends Message implements ISubscriptions {
        List: string[];
    }

    export interface ITopics extends IMessage {
        List: string[];
    }

    export class Topics extends Message implements ITopics {
        List: string[];
    }

}