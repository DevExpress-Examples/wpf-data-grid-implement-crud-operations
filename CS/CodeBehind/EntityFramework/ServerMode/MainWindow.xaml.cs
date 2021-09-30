using System.Windows;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using EntityFrameworkIssues.Issues;
using DevExpress.Mvvm.Xpf;
using System;
using System.Collections;

namespace EntityFrameworkIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            var context = new EntityFrameworkIssues.Issues.IssuesContext();
            var source = new DevExpress.Data.Linq.EntityServerModeSource
            {
                KeyExpression = nameof(EntityFrameworkIssues.Issues.Issue.Id),
                QueryableSource = context.Issues.AsNoTracking()
            };
            grid.ItemsSource = source;
            LoadLookupData();
        }

        void LoadLookupData() {
            var context = new EntityFrameworkIssues.Issues.IssuesContext();
            usersLookup.ItemsSource = context.Users.Select(user => new { Id = user.Id, Name = user.FirstName + " " + user.LastName }).ToArray();
        }

        void OnRefresh(System.Object sender, DevExpress.Xpf.Grid.RefreshEventArgs e) {
            LoadLookupData();
        }

        void OnCreateEditEntityViewModel(System.Object sender, DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs e) {
            var context = new IssuesContext();
            Issue item;
            if(e.Key != null)
                item = context.Issues.Find(e.Key);
            else {
                item = new Issue() { Created = DateTime.Now };
                context.Entry(item).State = EntityState.Added;
            }
            e.ViewModel = new EditItemViewModel(item, new EditIssueInfo(context, (IList)usersLookup.ItemsSource));
        }

        void OnValidateRow(System.Object sender, DevExpress.Mvvm.Xpf.EditFormRowValidationArgs e) {
            var context = ((EditIssueInfo)e.Tag).Context;
            context.SaveChanges();
        }

        void OnValidateDeleteRows(System.Object sender, DevExpress.Mvvm.Xpf.EditFormDeleteRowsValidationArgs e) {
            var key = (int)e.Keys.Single();
            var item = new Issue() { Id = key };
            var context = new IssuesContext();
            context.Entry(item).State = EntityState.Deleted;
            context.SaveChanges();
        }
    }
}
