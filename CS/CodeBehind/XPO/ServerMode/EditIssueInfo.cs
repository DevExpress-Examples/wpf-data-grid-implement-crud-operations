using DevExpress.Mvvm;
using System.Collections;
using XPOIssues.Issues;

namespace XPOIssues {
    public class EditIssueInfo : BindableBase {
        public EditIssueInfo(DevExpress.Xpo.UnitOfWork unitOfWork, IList users) {
            UnitOfWork = unitOfWork;
            Users = users;
        }
        public DevExpress.Xpo.UnitOfWork UnitOfWork { get; }
        public IList Users { get; }
    }
}