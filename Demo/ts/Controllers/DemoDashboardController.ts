/// <references path="../all.ts" />

module Demo {

    export interface IDemoDashboardScope extends ng.IScope {
        location: ng.ILocationService;
        vm: DemoDashboardController;
    }

    export class DemoDashboardController {
        logs: string[];
        topics: string[];
        subscribers: string[];

        public static $inject = [
            '$scope',
            '$location',
            'ws'
        ];

        constructor(
            private $scope: IDemoDashboardScope,
            private $location: ng.ILocationService,
            private ws: ISubscriptionScope
            ) {
            $scope.vm = this;
            ws.vm.OnOpen = this.onOpen;
            ws.vm.OnClose = this.onClose;
            ws.vm.OnMessage = this.onMessage;

            // add watchers for path changes
            $scope.$watch('location.path()', path => this.onPath(path));
            if ($location.path() === '') $location.path('/');
            $scope.location = $location;
        }

        onPath(path: string) {
            if (path === '/open')
                this.ws.vm.Open();
            else if (path === '/close')
                this.ws.vm.Close();
            else if (path === '/subscribe')
                this.ws.vm.Subscribe(typeof PubSub.Messages.TopicUpdate, this.onUpdate);
            else if (path === '/unsubscribe')
                this.ws.vm.Unsubscribe(typeof PubSub.Messages.TopicUpdate, this.onUpdate);
        }

        // TODO: determine if this could move to within PubSub module (is it generic enough?)
        private onUpdate(message: PubSub.Messages.TopicUpdate) {

        }

        private onOpen() {
            this.logs.push(Date.now + ": socket opened");
        }

        private onClose() {
            this.logs.push(Date.now + ": socket closed");
        }

        private onError(e: string) {
            this.logs.push(Date.now + ": socket error: " + e);
        }

        private onMessage(m: string) {
            this.logs.push(Date.now + ": received message, count=" + m.length);
        }
    }
}