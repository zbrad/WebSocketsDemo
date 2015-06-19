module WebDemo {

    export class WsCtrl {

        public static $inject = [
            '$scope',
            '$location',
            'WsSvc',
        ];

        constructor(
            private $scope: IWsScope,
            private $location: ng.ILocationService,
            private wsSvc: IWsSvc
            ) {


        }


    }


}