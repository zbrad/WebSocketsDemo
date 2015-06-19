var WebDemo;
(function (WebDemo) {
    var wsmvc = angular.module('wsmvc', [])
        .controller('wsCtrl', WsCtrl)
        .service('wsSvc', WsSvc);
})(WebDemo || (WebDemo = {}));
