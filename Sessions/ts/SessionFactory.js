/// <reference path="../typings/all.d.ts"/>
var Sessions;
(function (Sessions) {
    var SessionFactory = (function () {
        function SessionFactory() {
        }
        SessionFactory.prototype.Add = function (session) {
            this.sessions[session.Id] = session;
        };
        SessionFactory.prototype.Remove = function (session) {
            this.sessions[session.Id] = null;
        };
        return SessionFactory;
    })();
})(Sessions || (Sessions = {}));
