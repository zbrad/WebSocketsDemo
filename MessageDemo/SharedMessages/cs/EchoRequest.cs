using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageLib;

namespace SharedMessages
{
    public class EchoRequest : Message
    {
        public string Text { get; set; }

        public EchoRequest() { }
        public EchoRequest(string text) { this.Text = text; }

    }
}
