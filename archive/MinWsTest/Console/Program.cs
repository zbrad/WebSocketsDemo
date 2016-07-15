using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Net.WebSockets;

namespace MonitorTest
{
    public class Program
    {
        WebSocket socket;

        public void Main(string[] args)
        {
            GetConnected();
            RunConnected();
        }

        static string linePat = @"(?<command>[se])\w*\s*(?<content>.*$)?";
        static System.Text.RegularExpressions.Regex lineRegex = new System.Text.RegularExpressions.Regex(linePat);
        static string defaultEndpoint = "ws://localhost:57762/ws";

        void GetConnected()
        {
            while (true)
            {
                Console.Write("Endpoint[" + defaultEndpoint + "]: ");
                var endpoint = Console.ReadLine();
                if (string.IsNullOrEmpty(endpoint))
                    endpoint = defaultEndpoint;

                try
                {
                    var ws = new ClientWebSocket();
                    ws.ConnectAsync(new Uri(endpoint), CancellationToken.None).Wait();
                    this.socket = ws;
                }
                catch (Exception e)
                {
                    Console.WriteLine("?connect failed: " + e.Message);
                    continue;
                }

                if (socket != null)
                    break;

                Console.WriteLine("?failed to connect to: " + endpoint);
            }

            Task.Run(SocketListener);
        }

        private async Task SocketListener()
        {
            byte[] buffer = new byte[4096];
            var seg = new ArraySegment<byte>(buffer);
            while (this.socket != null && this.socket.State == WebSocketState.Open)
            {
                var result = await this.socket.ReceiveAsync(seg, CancellationToken.None);
                if (result.CloseStatus.HasValue)
                {
                    Console.WriteLine("socket closed");
                    return;
                }

                var message = UTF8Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("Received message: " + message);
            }
        }

        void RunConnected()
        {
            while (socket != null && socket.State == WebSocketState.Open)
            {
                Console.WriteLine(@"
Options:
  [s]end message
  [e]xit
");
                var line = Console.ReadLine();
                var m = lineRegex.Match(line);
                if (!m.Success)
                {
                    Console.WriteLine("?Could not match command: " + line);
                    continue;
                }

                var command = m.Groups["command"].Value;
                switch (command)
                {
                    case "s":
                        {
                            var content = m.Groups["content"].Value;
                            var bytes = UTF8Encoding.UTF8.GetBytes(content);
                            var seg = new ArraySegment<byte>(bytes);
                            socket.SendAsync(seg, WebSocketMessageType.Binary, true, CancellationToken.None).Wait();
                            break;
                        }
                    case "e":
                        Console.WriteLine("Exiting...");
                        socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "client exit", CancellationToken.None).Wait();
                        return;
                    default:
                        throw new ApplicationException("unhandled command: " + command);
                }

            }

        }




    }
}
