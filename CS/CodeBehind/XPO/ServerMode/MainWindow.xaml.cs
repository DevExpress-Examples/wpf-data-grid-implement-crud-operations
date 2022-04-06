using System.Windows;
using XPOIssues.Issues;
using DevExpress.Xpo;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm.Xpf;
using System;
using System.Collections;

namespace XPOIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            var properties = new ServerViewProperty[] {
new ServerViewProperty("Subject", SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("Subject")),
new ServerViewProperty("UserId", SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("UserId")),
new ServerViewProperty("Created", SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("Created")),
new ServerViewProperty("Votes", SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("Votes")),
new ServerViewProperty("Priority", SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("Priority")),
new ServerViewProperty("Oid", SortDirection.Ascending, new DevExpress.Data.Filtering.OperandProperty("Oid"))
};
            var session = new Session();
            var source = new XPServerModeView(session, typeof(Issue), null);
            source.Properties.AddRange(properties);
            grid.ItemsSource = source;
            LoadLookupData();
        }

        void LoadLookupData() {
            var session = new DevExpress.Xpo.Session();
            usersLookup.ItemsSource = session.Query<XPOIssues.Issues.User>().OrderBy(user => user.Oid).Select(user => new { Id = user.Oid, Name = user.FirstName + " " + user.LastName }).ToArray();
        }
        void OnDataSourceRefresh(object sender, DevExpress.Xpf.Grid.DataSourceRefreshEventArgs e) {
            LoadLookupData();
        }
        void OnCreateEditEntityViewModel(object sender, DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs e) {
            var unitOfWork = new UnitOfWork();
            var item = e.IsNewItem
                ? new Issue(unitOfWork)
                : unitOfWork.GetObjectByKey<Issue>(e.Key);
            e.ViewModel = new EditItemViewModel(
                item,
                new EditIssueInfo(unitOfWork, (IList)usersLookup.ItemsSource),
                dispose: () => unitOfWork.Dispose(),
                title: (e.IsNewItem ? "New " : "Edit ") + nameof(Issue)
            );
        }
        void OnValidateRow(object sender, DevExpress.Mvvm.Xpf.EditFormRowValidationArgs e) {
            var unitOfWork = ((EditIssueInfo)e.EditOperationContext).UnitOfWork;
            unitOfWork.CommitChanges();
        }
        void OnValidateRowDeletion(object sender, DevExpress.Mvvm.Xpf.EditFormValidateRowDeletionArgs e) {
            using(var unitOfWork = new UnitOfWork()) {
                var key = (int)e.Keys.Single();
                var item = unitOfWork.GetObjectByKey<Issue>(key);
                unitOfWork.Delete(item);
                unitOfWork.CommitChanges();
            }
        }
    }
}
