using System.Windows;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo;
using XPOIssues.Issues;
using DevExpress.Mvvm.Xpf;
using System;
using System.Collections;

namespace XPOIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            var properties = new DevExpress.Xpo.ServerViewProperty[] {
new DevExpress.Xpo.ServerViewProperty("Oid", DevExpress.Xpo.SortDirection.Ascending, new DevExpress.Data.Filtering.OperandProperty("Oid")),
new DevExpress.Xpo.ServerViewProperty("Subject", DevExpress.Xpo.SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("Subject")),
new DevExpress.Xpo.ServerViewProperty("UserId", DevExpress.Xpo.SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("UserId")),
new DevExpress.Xpo.ServerViewProperty("Created", DevExpress.Xpo.SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("Created")),
new DevExpress.Xpo.ServerViewProperty("Votes", DevExpress.Xpo.SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("Votes")),
new DevExpress.Xpo.ServerViewProperty("Priority", DevExpress.Xpo.SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("Priority"))
};
            var session = new DevExpress.Xpo.Session();
            var source = new DevExpress.Xpo.XPServerModeView(session, typeof(XPOIssues.Issues.Issue), null);
            source.Properties.AddRange(properties);
            grid.ItemsSource = source;
            LoadLookupData();
        }

        void LoadLookupData() {
            var session = new DevExpress.Xpo.Session();
            usersLookup.ItemsSource = session.Query<XPOIssues.Issues.User>().OrderBy(user => user.Oid).Select(user => new { Id = user.Oid, Name = user.FirstName + " " + user.LastName }).ToArray();
        }

        void OnDataSourceRefresh(System.Object sender, DevExpress.Xpf.Grid.DataSourceRefreshEventArgs e) {
            LoadLookupData();
        }

        void OnCreateEditEntityViewModel(System.Object sender, DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs e) {
            var unitOfWork = new UnitOfWork();
            var item = e.Key != null
                ? unitOfWork.GetObjectByKey<Issue>(e.Key)
                : new Issue(unitOfWork);
            e.ViewModel = new EditItemViewModel(
                item,
                new EditIssueInfo(unitOfWork, (IList)usersLookup.ItemsSource),
                dispose: () => unitOfWork.Dispose()
            );
        }

        void OnValidateRow(System.Object sender, DevExpress.Mvvm.Xpf.EditFormRowValidationArgs e) {
            var unitOfWork = ((EditIssueInfo)e.Tag).UnitOfWork;
            unitOfWork.CommitChanges();
        }

        void OnValidateRowDeletion(System.Object sender, DevExpress.Mvvm.Xpf.EditFormDeleteRowsValidationArgs e) {
            using(var unitOfWork = new UnitOfWork()) {
                var key = (int)e.Keys.Single();
                var item = unitOfWork.GetObjectByKey<Issue>(key);
                unitOfWork.Delete(item);
                unitOfWork.CommitChanges();
            }
        }
    }
}
