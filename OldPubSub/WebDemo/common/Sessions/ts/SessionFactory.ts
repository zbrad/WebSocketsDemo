module WebDemo {

    class SessionFactory implements ISessionFactory {
        private sessions: { [type: string]: ISession };

        Add(session: ISession) {
            this.sessions[session.Id] = session;
        }

        Remove(session: ISession) {
            this.sessions[session.Id] = null;
        }
    }
}
