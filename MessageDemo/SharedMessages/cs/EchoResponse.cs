using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageLib;

namespace SharedMessages
{
    public class EchoResponse : Message
    {
        public string Text { get; set; }

        public EchoResponse() { }

        public EchoResponse(string text) { this.Text = text; }
    }
}
