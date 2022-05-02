using DevExpress.Xpo;
using DevExpress.Mvvm;
using DevExpress.Xpf.Grid;
using System.Collections;
using XPOIssues.Issues;

namespace XPOIssues {
    public class EditIssueInfo : BindableBase {
        public EditIssueInfo(UnitOfWork unitOfWork, IList users) {
            UnitOfWork = unitOfWork;
            Users = users;
        }
        public UnitOfWork UnitOfWork { get; }
        public IList Users { get; }
    }
}