using DevExpress.Mvvm;
using DevExpress.Xpf.Grid;
using System.Collections;
using EFCoreIssues.Issues;

namespace EFCoreIssues {
    public class EditIssueInfo : BindableBase {
        public EditIssueInfo(IssuesContext dbContext, IList users) {
            DbContext = dbContext;
            Users = users;
        }
        public IssuesContext DbContext { get; }
        public IList Users { get; }
    }
}