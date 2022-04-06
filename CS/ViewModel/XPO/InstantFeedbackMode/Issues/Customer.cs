using DevExpress.Xpo;

namespace XPOIssues.Issues {
    public class User : XPObject {
        public User(Session session) : base(session) { }

        string _FirstName;
        public string FirstName {
            get { return _FirstName; }
            set { SetPropertyValue(nameof(FirstName), ref _FirstName, value); }
        }

        string _LastName;
        public string LastName {
            get { return _LastName; }
            set { SetPropertyValue(nameof(LastName), ref _LastName, value); }
        }

        [Association("UserIssues")]
        public XPCollection<Issue> Issues{
            get { return GetCollection<Issue>(nameof(Issues)); }
        }
    }
}
