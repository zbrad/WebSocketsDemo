using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MessageLib
{
    public class MessageConverter : JsonConverter
    {
        Dictionary<string, Type> knownTypes = new Dictionary<string, Type>();

        public void Add<T>() where T : Message
        {
            string name = typeof(T).Name;
            knownTypes[name] = typeof(T);
        }

        public override bool CanWrite { get { return false; } }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Message);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject o = JObject.Load(reader);

            JToken token;
            Type type;
            if (o.TryGetValue("$type", out token))
                type = Type.GetType(token.Value<string>());
            else
            {
                if (!o.TryGetValue("Type", out token))
                    return null;

                if (!knownTypes.TryGetValue(token.Value<string>(), out type))
                    return null;
            }

            try
            {
                return o.ToObject(type);
            }
            catch { return null; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}
