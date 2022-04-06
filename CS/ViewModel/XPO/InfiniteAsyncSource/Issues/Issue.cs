using System;
using DevExpress.Xpo;

namespace XPOIssues.Issues {
    public class Issue : XPObject {
        public Issue(Session session) : base(session) {
            Created = DateTime.Now;
        }
        string _Subject;
        [Size(200)]
        public string Subject {
            get { return _Subject; }
            set { SetPropertyValue(nameof(Subject), ref _Subject, value); }
        }

        int _UserId;
        public int UserId {
            get { return _UserId; }
            set { SetPropertyValue(nameof(UserId), ref _UserId, value); }
        }

        User _User;
        [Association("UserIssues")]
        public User User {
            get { return _User; }
            set { SetPropertyValue(nameof(User), ref _User, value); }
        }

        DateTime _Created;
        public DateTime Created {
            get { return _Created; }
            set { SetPropertyValue(nameof(Created), ref _Created, value); }
        }

        int _Votes;
        public int Votes {
            get { return _Votes; }
            set { SetPropertyValue(nameof(Votes), ref _Votes, value); }
        }

        Priority _Priority;
        public Priority Priority {
            get { return _Priority; }
            set { SetPropertyValue(nameof(Priority), ref _Priority, value); }
        }
    }

    public enum Priority { Low, BelowNormal, Normal, AboveNormal, High }
}
