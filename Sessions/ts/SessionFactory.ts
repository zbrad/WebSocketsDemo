/// <reference path="../typings/all.d.ts"/>

module Sessions {

    class SessionFactory implements S.ISessionFactory {
        private sessions: { [type: string]: S.ISession };

        Add(session: S.ISession) {
            this.sessions[session.Id] = session;
        }

        Remove(session: S.ISession) {
            this.sessions[session.Id] = null;
        }
    }
}
