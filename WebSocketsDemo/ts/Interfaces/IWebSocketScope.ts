/// <references path="../all.ts" />

module WebSocketsDemo {

    export interface ISubscriptionScope extends ng.IScope {
        location: ng.ILocationService;
        vm: SubscriptionController;
    }


}