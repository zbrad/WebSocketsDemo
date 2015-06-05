/// <reference path="all.ts" />
//
// the main angular mvc module
//
var WebSocketsDemo;
(function (WebSocketsDemo) {
    var app = angular.module('app', ['luegg.directives'])
        .controller('WebSocketController', WebSocketsDemo.SubscriptionController);
})(WebSocketsDemo || (WebSocketsDemo = {}));
