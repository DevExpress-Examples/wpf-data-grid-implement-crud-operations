using System.Windows;
using XPOIssues.Issues;
using DevExpress.Xpo;
using System.Linq;
using DevExpress.Xpf.Grid;

namespace XPOIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            LoadData();
        }
        UnitOfWork _UnitOfWork;

        void LoadData() {
            _UnitOfWork = new UnitOfWork();
            var xpCollection = new XPCollection<User>(_UnitOfWork);
            xpCollection.Sorting.Add(new SortProperty(nameof(User.Oid), DevExpress.Xpo.DB.SortingDirection.Ascending));
            grid.ItemsSource = xpCollection;
        }
        void OnValidateRow(object sender, GridRowValidationEventArgs e) {
            _UnitOfWork.CommitChanges();
        }
        void OnValidateRowDeletion(object sender, GridValidateRowDeletionEventArgs e) {
            var row = (User)e.Rows.Single();
            _UnitOfWork.Delete(row);
            _UnitOfWork.CommitChanges();
        }
        void OnDataSourceRefresh(object sender, DataSourceRefreshEventArgs e) {
            LoadData();
        }
    }
}
