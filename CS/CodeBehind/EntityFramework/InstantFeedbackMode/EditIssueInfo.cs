using DevExpress.Mvvm;
using System.Collections;
using EntityFrameworkIssues.Issues;

namespace EntityFrameworkIssues {
    public class EditIssueInfo : BindableBase {
        public EditIssueInfo(IssuesContext context, IList users) {
            Context = context;
            Users = users;
        }
        public IssuesContext Context { get; }
        public IList Users { get; }
    }
}