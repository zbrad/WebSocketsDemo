using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;

namespace WsClient
{
    class Program
    {
        //const string AppUrl = "http://localhost:55622/";
        const string AppUrl = "http://localhost:5000/";

        static void Main(string[] args)
        {
            var appUrl = new Uri(AppUrl);
            var endpoint = new Uri("ws://localhost:" + appUrl.Port + "/api/session");

            Console.WriteLine("Endpoint=" + endpoint);
            var wsc = new ClientWebSocket();
            wsc.ConnectAsync(endpoint, CancellationToken.None).GetAwaiter().GetResult();

            while (true)
            {
                Console.Write("Input: ");
                var line = Console.ReadLine();
                var testSeg = new ArraySegment<byte>(Encoding.UTF8.GetBytes(line));
                wsc.SendAsync(testSeg, WebSocketMessageType.Binary, true, CancellationToken.None).GetAwaiter().GetResult();

                var buffer = new byte[1024];
                var recSeg = new ArraySegment<byte>(buffer);
                var result = wsc.ReceiveAsync(recSeg, CancellationToken.None).GetAwaiter().GetResult();

                var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("received: " + text);
            }

        }
    }
}
