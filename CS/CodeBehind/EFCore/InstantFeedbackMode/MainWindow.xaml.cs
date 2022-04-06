using System.Windows;
using EFCoreIssues.Issues;
using Microsoft.EntityFrameworkCore;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm.Xpf;
using System;
using System.Collections;

namespace EFCoreIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            var source = new DevExpress.Data.Linq.EntityInstantFeedbackSource
            {
                KeyExpression = nameof(Issue.Id)
            };
            source.GetQueryable += (sender, e) =>
            {
                var context = new IssuesContext();
                e.QueryableSource = context.Issues.AsNoTracking();
            };
            grid.ItemsSource = source;
            LoadLookupData();
        }

        void LoadLookupData() {
            var context = new EFCoreIssues.Issues.IssuesContext();
            usersLookup.ItemsSource = context.Users.Select(user => new { Id = user.Id, Name = user.FirstName + " " + user.LastName }).ToArray();
        }
        void OnDataSourceRefresh(object sender, DevExpress.Xpf.Grid.DataSourceRefreshEventArgs e) {
            LoadLookupData();
        }
        void OnCreateEditEntityViewModel(object sender, DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs e) {
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
        void OnValidateRow(object sender, DevExpress.Mvvm.Xpf.EditFormRowValidationArgs e) {
            var context = ((EditIssueInfo)e.EditOperationContext).DbContext;
            context.SaveChanges();
        }
        void OnValidateRowDeletion(object sender, DevExpress.Mvvm.Xpf.EditFormValidateRowDeletionArgs e) {
            var key = (int)e.Keys.Single();
            var item = new Issue() { Id = key };
            var context = new IssuesContext();
            context.Entry(item).State = EntityState.Deleted;
            context.SaveChanges();
        }
    }
}