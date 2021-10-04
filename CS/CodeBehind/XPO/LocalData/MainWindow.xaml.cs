using System.Windows;
using System.Linq;

namespace XPOIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            LoadData();
        }
        DevExpress.Xpo.UnitOfWork _UnitOfWork;

        void LoadData() {
            _UnitOfWork = new DevExpress.Xpo.UnitOfWork();
            var xpCollection = new DevExpress.Xpo.XPCollection<XPOIssues.Issues.User>(_UnitOfWork);
            xpCollection.Sorting.Add(new DevExpress.Xpo.SortProperty(nameof(XPOIssues.Issues.User.Oid), DevExpress.Xpo.DB.SortingDirection.Ascending));
            grid.ItemsSource = xpCollection;
        }

        void OnValidateRow(System.Object sender, DevExpress.Xpf.Grid.GridRowValidationEventArgs e) {
            _UnitOfWork.CommitChanges();
        }

        void OnValidateDeleteRows(System.Object sender, DevExpress.Xpf.Grid.GridDeleteRowsValidationEventArgs e) {
            var row = (XPOIssues.Issues.User)e.Rows.Single();
            _UnitOfWork.Delete(row);
            _UnitOfWork.CommitChanges();
        }

        void OnRefresh(System.Object sender, DevExpress.Xpf.Grid.RefreshEventArgs e) {
            LoadData();
        }
    }
}
