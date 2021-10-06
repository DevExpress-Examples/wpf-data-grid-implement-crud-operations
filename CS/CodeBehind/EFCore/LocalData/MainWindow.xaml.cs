using System.Windows;
using System.Linq;

namespace EFCoreIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            LoadData();
        }
        EFCoreIssues.Issues.IssuesContext _Context;

        void LoadData() {
            _Context = new EFCoreIssues.Issues.IssuesContext();
            grid.ItemsSource = _Context.Users.ToList();
        }

        void OnValidateRow(System.Object sender, DevExpress.Xpf.Grid.GridRowValidationEventArgs e) {
            var row = (EFCoreIssues.Issues.User)e.Row;
            if(e.IsNewItem)
                _Context.Users.Add(row);
            _Context.SaveChanges();
        }

        void OnValidateDeleteRows(System.Object sender, DevExpress.Xpf.Grid.GridDeleteRowsValidationEventArgs e) {
            var row = (EFCoreIssues.Issues.User)e.Rows.Single();
            _Context.Users.Remove(row);
            _Context.SaveChanges();
        }

        void OnRefreshDataSource(System.Object sender, DevExpress.Xpf.Grid.RefreshDataSourceEventArgs e) {
            LoadData();
        }
    }
}
