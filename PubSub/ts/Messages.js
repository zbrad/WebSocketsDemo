// <reference path="../Sessions/lib/Sessions.d.ts" />
// <reference path="../Sessions/lib/Messages.d.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var PubSub;
(function (PubSub) {
    var Subscribe = (function (_super) {
        __extends(Subscribe, _super);
        function Subscribe() {
            _super.apply(this, arguments);
        }
        return Subscribe;
    })(Message);
    PubSub.Subscribe = Subscribe;
    var Unsubscribe = (function (_super) {
        __extends(Unsubscribe, _super);
        function Unsubscribe() {
            _super.apply(this, arguments);
        }
        return Unsubscribe;
    })(Sessions.Message);
    PubSub.Unsubscribe = Unsubscribe;
    var TopicUpdate = (function (_super) {
        __extends(TopicUpdate, _super);
        function TopicUpdate() {
            _super.apply(this, arguments);
        }
        return TopicUpdate;
    })(Sessions.Message);
    PubSub.TopicUpdate = TopicUpdate;
    var GetSubscriptions = (function () {
        function GetSubscriptions() {
        }
        return GetSubscriptions;
    })();
    PubSub.GetSubscriptions = GetSubscriptions;
    var GetTopics = (function () {
        function GetTopics() {
        }
        return GetTopics;
    })();
    PubSub.GetTopics = GetTopics;
    var Subscriptions = (function () {
        function Subscriptions() {
        }
        return Subscriptions;
    })();
    PubSub.Subscriptions = Subscriptions;
    var Topics = (function () {
        function Topics() {
        }
        return Topics;
    })();
    PubSub.Topics = Topics;
})(PubSub || (PubSub = {}));
