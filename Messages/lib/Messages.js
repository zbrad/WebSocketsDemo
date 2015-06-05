// <references path="../typings/all.d.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Messages;
(function (Messages) {
    var Message = (function () {
        function Message() {
            this.$type = this.Type = typeof this;
        }
        return Message;
    })();
    Messages.Message = Message;
    var Error = (function (_super) {
        __extends(Error, _super);
        function Error() {
            _super.apply(this, arguments);
        }
        return Error;
    })(Message);
    Messages.Error = Error;
})(Messages || (Messages = {}));
//# sourceMappingURL=Messages.js.map