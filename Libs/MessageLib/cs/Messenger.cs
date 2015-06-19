using System;
using System.Threading;
using System.Threading.Tasks;
using SessionLib;

namespace MessageLib
{
    internal abstract class Messenger : IMessenger
    {
        public event MessageHandler OnMessage;

        public ISession Session { get; private set; }
        public bool IsClosed { get; private set; }
        public Exception LastException { get; protected set; }
        public MessengerFactory Factory { get; private set; }

        public Messenger(IMessengerFactory factory, ISession session)
        {
            this.Factory = (MessengerFactory) factory;
            this.Session = session;
            this.Session.OnReceive += Session_OnReceive;
        }

        private async Task Session_OnReceive(ISession session, ArraySegment<byte> m)
        {
            var on = this.OnMessage;
            if (on != null)
            {
                try
                {
                    var message = Message.Deserialize(m);
                    await on(this, message);
                }
                catch (Exception e)
                {
                    this.LastException = e;
                }
            }
        }

        public Task WaitAsync()
        {
            return Session.WaitAsync();
        }

        public Task SendAsync(Message m)
        {
            return this.SendAsync(m, CancellationToken.None);
        }

        public Task SendAsync(Message m, CancellationToken token)
        {
            var bytes = m.Serialize();
            var seg = new ArraySegment<byte>(bytes);
            return this.Session.SendAsync(seg, token);
        }

        public Task CloseAsync()
        {
            return this.Session.CloseAsync();
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