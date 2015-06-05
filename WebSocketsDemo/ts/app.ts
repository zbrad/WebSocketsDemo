/// <reference path="all.ts" />

//
// the main angular mvc module
//
module WebSocketsDemo {
    var app = angular.module('app', ['luegg.directives'])
        .controller('WebSocketController', SubscriptionController);
}