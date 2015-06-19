module WebDemo {

    export interface IWsScope extends ng.IScope {
        location: ng.ILocationService;
        vm: WsCtrl;
    }


}