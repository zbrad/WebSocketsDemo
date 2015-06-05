/// <references path="../all.ts" />
var WebSocketsDemo;
(function (WebSocketsDemo) {
    var DemoDashboardController = (function () {
        function DemoDashboardController($scope, $location, ws) {
            var _this = this;
            this.$scope = $scope;
            this.$location = $location;
            this.ws = ws;
            $scope.vm = this;
            ws.vm.OnOpen = this.onOpen;
            ws.vm.OnClose = this.onClose;
            ws.vm.OnMessage = this.onMessage;
            // add watchers for path changes
            $scope.$watch('location.path()', function (path) { return _this.onPath(path); });
            if ($location.path() === '')
                $location.path('/');
            $scope.location = $location;
        }
        DemoDashboardController.prototype.onPath = function (path) {
            if (path === '/open')
                this.ws.vm.Open();
            else if (path === '/close')
                this.ws.vm.Close();
            else if (path === '/subscribe')
                this.ws.vm.Subscribe(typeof PubSub.Messages.TopicUpdate, this.onUpdate);
            else if (path === '/unsubscribe')
                this.ws.vm.Unsubscribe(typeof PubSub.Messages.TopicUpdate, this.onUpdate);
        };
        // TODO: determine if this could move to within PubSub module (is it generic enough?)
        DemoDashboardController.prototype.onUpdate = function (message) {
        };
        DemoDashboardController.prototype.onOpen = function () {
            this.logs.push(Date.now + ": socket opened");
        };
        DemoDashboardController.prototype.onClose = function () {
            this.logs.push(Date.now + ": socket closed");
        };
        DemoDashboardController.prototype.onError = function (e) {
            this.logs.push(Date.now + ": socket error: " + e);
        };
        DemoDashboardController.prototype.onMessage = function (m) {
            this.logs.push(Date.now + ": received message, count=" + m.length);
        };
        DemoDashboardController.$inject = [
            '$scope',
            '$location',
            'ws'
        ];
        return DemoDashboardController;
    })();
    WebSocketsDemo.DemoDashboardController = DemoDashboardController;
})(WebSocketsDemo || (WebSocketsDemo = {}));
