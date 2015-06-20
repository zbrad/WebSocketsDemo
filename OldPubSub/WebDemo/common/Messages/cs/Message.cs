using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Messages
{

    public abstract class Message
    {
        public static readonly MessageConverter Converter = new MessageConverter();
        public static readonly JsonSerializerSettings SerializerSettings =
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                Converters = new[] { Converter }
            };

        public string Type { get; set; }

        public Message()
        {
            this.Type = this.GetType().Name;
        }

        public byte[] Serialize()
        {
            try
            {
                string s = JsonConvert.SerializeObject(this, SerializerSettings);
                byte[] b = UTF8Encoding.UTF8.GetBytes(s);
                return b;
            }
            catch
            {
                return null;
            }
        }

        public static Message Deserialize(string content)
        {
            try
            {
                return JsonConvert.DeserializeObject<Message>(content, Message.SerializerSettings);
            }
            catch
            {
                return null;
            }

        }

        public static Message Deserialize(byte[] bytes)
        {
            string s = UTF8Encoding.UTF8.GetString(bytes);
            return Deserialize(s);
        }

        public static Message Deserialize(ArraySegment<byte> segment)
        {
            var s = UTF8Encoding.UTF8.GetString(segment.Array, 0, segment.Count);
            return Deserialize(s);
        }
    }
}