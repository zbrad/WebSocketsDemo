using System.Threading;
using System.Threading.Tasks;
using Sessions;

namespace PubSub
{

    internal class PublisherSession : IPublisherSession
    {
        ISession session;
        Publisher publisher;

        public PublisherSession(IPublisher publisher, ISession session)
        {
            this.publisher = (Publisher) publisher;
            this.session = session;
            this.publisher.Add(session);
        }

        public async Task WaitAsync(CancellationToken token)
        {
            await session.WaitAsync(token);
        }
    }
}
