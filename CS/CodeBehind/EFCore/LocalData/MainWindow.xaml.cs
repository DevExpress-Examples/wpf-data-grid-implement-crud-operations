using System.Windows;
using System.Linq;

namespace EFCoreIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Refresh();
        }
        EFCoreIssues.Issues.IssuesContext _Context;

        void Refresh() {
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

        void OnRefresh(System.Object sender, DevExpress.Xpf.Grid.RefreshEventArgs e) {
            Refresh();
        }
    }
}
