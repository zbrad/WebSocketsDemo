using System.Threading;
using System.Threading.Tasks;
using Sessions;

namespace PubSub
{

    internal class PublisherSession : IPublisherSession
    {
        IServerSession session;
        IPublisher publisher;

        public PublisherSession(IPublisher publisher, IServerSession session)
        {
            this.publisher = publisher;
            this.session = session;
            publisher.Add(session);
        }

        public async Task WaitAsync(CancellationToken token)
        {
            await session.WaitAsync(token);
        }
    }
}
