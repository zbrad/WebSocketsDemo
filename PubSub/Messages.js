var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var PubSub;
(function (PubSub) {
    var Messages;
    (function (Messages) {
        var Message = (function () {
            function Message() {
                this.Type = typeof this;
            }
            return Message;
        })();
        Messages.Message = Message;
        var Subscribe = (function (_super) {
            __extends(Subscribe, _super);
            function Subscribe() {
                _super.apply(this, arguments);
            }
            return Subscribe;
        })(Message);
        Messages.Subscribe = Subscribe;
        var TopicUpdate = (function (_super) {
            __extends(TopicUpdate, _super);
            function TopicUpdate() {
                _super.apply(this, arguments);
            }
            return TopicUpdate;
        })(Message);
        Messages.TopicUpdate = TopicUpdate;
        var Error = (function (_super) {
            __extends(Error, _super);
            function Error() {
                _super.apply(this, arguments);
            }
            return Error;
        })(Message);
        Messages.Error = Error;
        var GetSubscriptions = (function () {
            function GetSubscriptions() {
            }
            return GetSubscriptions;
        })();
        Messages.GetSubscriptions = GetSubscriptions;
        var Subscriptions = (function () {
            function Subscriptions() {
            }
            return Subscriptions;
        })();
        Messages.Subscriptions = Subscriptions;
    })(Messages = PubSub.Messages || (PubSub.Messages = {}));
})(PubSub || (PubSub = {}));
