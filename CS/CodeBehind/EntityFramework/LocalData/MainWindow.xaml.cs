using System.Windows;
using EntityFrameworkIssues.Issues;
using System.Data.Entity;
using System.Linq;
using DevExpress.Xpf.Grid;

namespace EntityFrameworkIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            LoadData();
        }
        IssuesContext _Context;

        void LoadData() {
            _Context = new IssuesContext();
            grid.ItemsSource = _Context.Users.ToList();
        }

        void OnValidateRow(object sender, GridRowValidationEventArgs e) {
            var row = (User)e.Row;
            if(e.IsNewItem)
                _Context.Users.Add(row);
            _Context.SaveChanges();
        }

        void OnValidateRowDeletion(object sender, GridValidateRowDeletionEventArgs e) {
            var row = (User)e.Rows.Single();
            _Context.Users.Remove(row);
            _Context.SaveChanges();
        }

        void OnDataSourceRefresh(object sender, DataSourceRefreshEventArgs e) {
            LoadData();
        }
    }
}
