var WebDemo;
(function (WebDemo) {
    var WsCtrl = (function () {
        function WsCtrl($scope, $location, wsSvc) {
            this.$scope = $scope;
            this.$location = $location;
            this.wsSvc = wsSvc;
        }
        WsCtrl.$inject = [
            '$scope',
            '$location',
            'WsSvc',
        ];
        return WsCtrl;
    })();
    WebDemo.WsCtrl = WsCtrl;
})(WebDemo || (WebDemo = {}));
