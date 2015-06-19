var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebDemo;
(function (WebDemo) {
    var Subscribe = (function (_super) {
        __extends(Subscribe, _super);
        function Subscribe() {
            _super.apply(this, arguments);
        }
        return Subscribe;
    })(WebDemo.Message);
    WebDemo.Subscribe = Subscribe;
    var Unsubscribe = (function (_super) {
        __extends(Unsubscribe, _super);
        function Unsubscribe() {
            _super.apply(this, arguments);
        }
        return Unsubscribe;
    })(WebDemo.Message);
    WebDemo.Unsubscribe = Unsubscribe;
    var TopicUpdate = (function (_super) {
        __extends(TopicUpdate, _super);
        function TopicUpdate() {
            _super.apply(this, arguments);
        }
        return TopicUpdate;
    })(WebDemo.Message);
    WebDemo.TopicUpdate = TopicUpdate;
    var GetSubscriptions = (function (_super) {
        __extends(GetSubscriptions, _super);
        function GetSubscriptions() {
            _super.apply(this, arguments);
        }
        return GetSubscriptions;
    })(WebDemo.Message);
    WebDemo.GetSubscriptions = GetSubscriptions;
    var GetTopics = (function (_super) {
        __extends(GetTopics, _super);
        function GetTopics() {
            _super.apply(this, arguments);
        }
        return GetTopics;
    })(WebDemo.Message);
    WebDemo.GetTopics = GetTopics;
    var Subscriptions = (function (_super) {
        __extends(Subscriptions, _super);
        function Subscriptions() {
            _super.apply(this, arguments);
        }
        return Subscriptions;
    })(WebDemo.Message);
    WebDemo.Subscriptions = Subscriptions;
    var Topics = (function (_super) {
        __extends(Topics, _super);
        function Topics() {
            _super.apply(this, arguments);
        }
        return Topics;
    })(WebDemo.Message);
    WebDemo.Topics = Topics;
})(WebDemo || (WebDemo = {}));
//# sourceMappingURL=Messages.js.map