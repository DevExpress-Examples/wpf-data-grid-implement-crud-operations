using DevExpress.Mvvm;
using System.Collections;
using EFCoreIssues.Issues;

namespace EFCoreIssues {
    public class EditIssueInfo : BindableBase {
        public EditIssueInfo(IssuesContext context, IList users) {
            Context = context;
            Users = users;
        }
        public IssuesContext Context { get; }
        public IList Users { get; }
    }
}