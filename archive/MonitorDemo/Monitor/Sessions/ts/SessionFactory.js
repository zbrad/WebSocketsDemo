var WebDemo;
(function (WebDemo) {
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
})(WebDemo || (WebDemo = {}));
//# sourceMappingURL=SessionFactory.js.map