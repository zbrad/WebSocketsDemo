﻿using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Sessions.Messages;
using Nito.AsyncEx;
using Microsoft.AspNet.WebSockets.Protocol;

namespace Sessions
{
    internal abstract class Session : ISession
    {
        public event MessageEventHandler ReceiveEvent;

        public string Id { get; private set; }

        public WebSocket Socket { get; protected set; }

        AsyncCollection<Message> receiveQ = new AsyncCollection<Message>();
        AsyncCollection<Message> sendQ = new AsyncCollection<Message>();

        CancellationTokenSource cts = new CancellationTokenSource();

        byte[] buffer;

        public bool IsClosed { get; private set; }
        public string CloseStatus { get; private set; }
        public string CloseDescription { get; private set; }
        public Exception LastException { get; protected set; }

        public SessionFactory Factory { get; private set; }

        public Task SocketInitialize { get; protected set; }

        public Session(ISessionFactory factory)
        {
            this.Factory = (SessionFactory) factory;

            this.Id = Guid.NewGuid().ToString();
            buffer = new byte[this.Factory.BufferSize];
            this.Factory.Add(this);
        }

        private async Task SocketReceiveAsync()
        {
            try
            {
                // now loop for any incoming socket messages
                while (Socket.State == WebSocketState.Open)
                {
                    var buf = new ArraySegment<byte>(buffer);
                    var result = await this.Socket.ReceiveAsync(buf, this.cts.Token);
                    if (IsSocketClosed(result))
                        return;

                    var received = new ArraySegment<byte>(buffer, 0, result.Count);
                    var msg = Message.Deserialize(received);
                    await receiveQ.AddAsync(msg);
                }
            }
            catch (Exception e)
            {
                this.LastException = e;
                await CloseAsync();
            }
        }

        private async Task MessageReceiveAsync()
        {
            try
            {

                while (!cts.IsCancellationRequested)
                {
                    var m = await receiveQ.TakeAsync(cts.Token);
                    var r = this.ReceiveEvent;
                    if (r != null)
                        r(this, m);
                }
            }
            catch (Exception e)
            {
                this.LastException = e;
                await CloseAsync();
            }
        }

        private async Task MessageSendAsync()
        {
            try
            {
                while (!cts.IsCancellationRequested)
                {
                    var m = await sendQ.TakeAsync(cts.Token);
                    var segment = new ArraySegment<byte>(m.Serialize());
                    await SendAsync(segment);
                }
            }
            catch (Exception e)
            {
                this.LastException = e;
                await CloseAsync();
            }
        }

        internal async Task SendAsync(ArraySegment<byte> segment)
        {
            await this.Socket.SendAsync(segment, WebSocketMessageType.Binary, true, this.cts.Token);
        }

        private bool IsSocketClosed(WebSocketReceiveResult result)
        {
            if (!result.CloseStatus.HasValue)
                return false;

            this.CloseStatus = Enum.GetName(typeof(WebSocketCloseStatus), result.CloseStatus);
            this.CloseDescription = result.CloseStatusDescription;
            return true;
        }

        public async Task SendAsync(Message m)
        {
            await sendQ.AddAsync(m);
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

        public async Task WaitAsync(CancellationToken token)
        {
            // start our worker tasks

            Task socketReceiver = Task.Run(SocketReceiveAsync);
            Task messageReceiver = Task.Run(MessageReceiveAsync);
            Task messageSender = Task.Run(MessageSendAsync);

            // wait for all workers to complete
            await Task.WhenAll(socketReceiver, messageReceiver, messageSender);
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