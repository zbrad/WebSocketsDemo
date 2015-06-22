module WebDemo {

    export class Subscribe extends Message {
			Topic: string;
	}

    export class Unsubscribe extends Message { 
			Topic: string;
	}

    export interface ITopicUpdate extends Message {
		Topic: string;
        Publisher: string;
        Date: Date;
        Content: string;
    }

    export class TopicUpdate extends TopicMessage implements ITopicUpdate {
		Topic: string;
        Publisher: string;
        Content: string;
        Date: Date;
    }

	export class GetSubscriptions extends Message { }

    export class GetTopics extends Message { }

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