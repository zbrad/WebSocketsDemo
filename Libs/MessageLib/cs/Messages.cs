using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageLib
{
    public class Error : Message
    {
        public string Message { get; set; }

        public Error() { }

        public Error(string message) { this.Message = message; }
    }
}
