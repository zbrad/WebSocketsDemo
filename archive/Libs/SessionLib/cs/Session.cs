using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace SessionLib
{
    internal abstract class Session : ISession
    {
        public event SessionHandler OnReceive;

        public string Id { get; private set; }

        WebSocket socket;
        public WebSocket Socket
        {
            get { return socket; }
            protected set
            {
                socket = value;
                StartTasks();
            }
        }
        public bool IsClosed { get; private set; }
        public string CloseStatus { get; private set; }
        public string CloseDescription { get; private set; }
        public Exception LastException { get; protected set; }
        public SessionFactory Factory { get; private set; }
        public Task SocketInitialize { get; protected set; }

        // async blocking send/receiver queues
        AsyncCollection<ArraySegment<byte>> receiveQ = new AsyncCollection<ArraySegment<byte>>();
        AsyncCollection<ArraySegment<byte>> sendQ = new AsyncCollection<ArraySegment<byte>>();

        // common cancellation for this session
        CancellationTokenSource cts = new CancellationTokenSource();

        // worker tasks
        Task socketReceiver;
        Task messageReceiver;
        Task messageSender;

        public Session(ISessionFactory factory)
        {
            this.Factory = (SessionFactory) factory;
            this.Id = Guid.NewGuid().ToString();
            this.Factory.Add(this);
        }


        void StartTasks()
        {
            // start our worker tasks
            socketReceiver = Task.Run(SocketReceiveAsync, cts.Token);
            messageReceiver = Task.Run(MessageReceiveAsync, cts.Token);
            messageSender = Task.Run(MessageSendAsync, cts.Token);
        }

        async Task SocketReceiveAsync()
        {
            try
            {
                // now loop for any incoming socket messages
                while (Socket.State == WebSocketState.Open)
                {
                    // because we add the receive buffer to the receiveQ, we will
                    // need a new buffer for every receive, since the user may not have
                    // processed all the incoming receives on the queue
                    var buffer = new byte[this.Factory.BufferSize];
                    var buf = new ArraySegment<byte>(buffer);
                    var result = await this.Socket.ReceiveAsync(buf, this.cts.Token);
                    if (IsSocketClosed(result))
                        return;

                    var received = new ArraySegment<byte>(buffer, 0, result.Count);
                    await receiveQ.AddAsync(received);
                }
            }
            catch (Exception e)
            {
                this.LastException = e;
                await CloseAsync();
            }
        }

        async Task MessageReceiveAsync()
        {
            try
            {

                while (!cts.IsCancellationRequested)
                {
                    var m = await receiveQ.TakeAsync(cts.Token);
                    var r = this.OnReceive;
                    if (r != null)
                        await r(this, m);
                }
            }
            catch (Exception e)
            {
                this.LastException = e;
                await CloseAsync();
            }
        }

        async Task MessageSendAsync()
        {
            try
            {
                while (!cts.IsCancellationRequested)
                {
                    var m = await sendQ.TakeAsync(cts.Token);
                    await this.Socket.SendAsync(m, WebSocketMessageType.Binary, true, this.cts.Token);
                }
            }
            catch (Exception e)
            {
                this.LastException = e;
                await CloseAsync();
            }
        }


        public async Task WaitAsync()
        {
            // wait for all workers to complete
            await Task.WhenAll(socketReceiver, messageReceiver, messageSender);
        }

        public Task SendAsync(ArraySegment<byte> m)
        {
            return this.SendAsync(m, CancellationToken.None);
        }

        public async Task SendAsync(ArraySegment<byte> m, CancellationToken token)
        {
            await sendQ.AddAsync(m, token);
        }

        public async Task CloseAsync()
        {
            this.IsClosed = true;

            if (this.Socket.State == WebSocketState.Open)
            {
                await this.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closed by call", CancellationToken.None);
                this.Socket.Dispose();
                this.Socket = null;
            }

            if (!this.cts.IsCancellationRequested)
                this.cts.Cancel();

            this.Factory.Remove(this);
        }

        bool IsSocketClosed(WebSocketReceiveResult result)
        {
            if (!result.CloseStatus.HasValue)
                return false;

            this.CloseStatus = Enum.GetName(typeof(WebSocketCloseStatus), result.CloseStatus);
            this.CloseDescription = result.CloseStatusDescription;
            return true;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        // allows inheritors to call base method and perform local cleanup
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CloseAsync().Wait();                    
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}