using System.Windows;
using EntityFrameworkIssues.Issues;
using DevExpress.Data.Linq;
using System.Data.Entity;
using System.Linq;
using DevExpress.Xpf.Grid;
using DevExpress.Mvvm.Xpf;
using System;
using System.Collections;

namespace EntityFrameworkIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            var context = new IssuesContext();
            var source = new EntityServerModeSource {
                KeyExpression = nameof(Issue.Id),
                QueryableSource = context.Issues.AsNoTracking()
            };
            grid.ItemsSource = source;
            LoadLookupData();
        }

        void LoadLookupData() {
            var context = new IssuesContext();
            usersLookup.ItemsSource = context.Users.Select(user => new { Id = user.Id, Name = user.FirstName + " " + user.LastName }).ToArray();
        }

        void OnDataSourceRefresh(object sender, DataSourceRefreshEventArgs e) {
            LoadLookupData();
        }

        void OnCreateEditEntityViewModel(object sender, CreateEditItemViewModelArgs e) {
            var context = new IssuesContext();
            Issue item;
            if(e.IsNewItem) {
                item = new Issue() { Created = DateTime.Now };
                context.Entry(item).State = EntityState.Added;
            } else {
                item = context.Issues.Find(e.Key);
            }
            e.ViewModel = new EditItemViewModel(
                item,
                new EditIssueInfo(context, (IList)usersLookup.ItemsSource),
                title: (e.IsNewItem ? "New " : "Edit ") + nameof(Issue)
            );
        }

        void OnValidateRow(object sender, EditFormRowValidationArgs e) {
            var context = ((EditIssueInfo)e.EditOperationContext).DbContext;
            context.SaveChanges();
        }

        void OnValidateRowDeletion(object sender, EditFormValidateRowDeletionArgs e) {
            var key = (int)e.Keys.Single();
            var item = new Issue() { Id = key };
            var context = new IssuesContext();
            context.Entry(item).State = EntityState.Deleted;
            context.SaveChanges();
        }
    }
}
