module WebDemo {
    var pubsubApp = angular.module('pubsubApp', ['ngRoute', 'pubsubControllers'])
        .controller('wsCtrl', WsCtrl)
        .service('wsSvc', WsSvc);





}