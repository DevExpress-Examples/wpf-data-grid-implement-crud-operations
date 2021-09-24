using System.Windows;
using System.Linq;
using EntityFrameworkIssues.Issues;
using DevExpress.Mvvm.Xpf;
using System.Data.Entity;
using System;
using System.Collections;

namespace EntityFrameworkIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            var source = new DevExpress.Data.Linq.EntityInstantFeedbackSource
            {
                KeyExpression = nameof(EntityFrameworkIssues.Issues.Issue.Id)
            };
            source.GetQueryable += (sender, e) =>
            {
                var context = new EntityFrameworkIssues.Issues.IssuesContext();
                e.QueryableSource = context.Issues.AsNoTracking();
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
