using System.Windows;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFCoreIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            var source = new PagedAsyncSource
            {
                ElementType = typeof(EFCoreIssues.Issues.Issue),
                KeyProperty = nameof(EFCoreIssues.Issues.Issue.Id),
                PageNavigationMode = PageNavigationMode.ArbitraryWithTotalPageCount
            };
            source.FetchPage += OnFetchPage;
            source.GetTotalSummaries += OnGetTotalSummaries;
            grid.ItemsSource = source;
            LoadLookupData();
        }

        void OnFetchPage(System.Object sender, DevExpress.Xpf.Data.FetchPageAsyncEventArgs e) {
            const int pageTakeCount = 5;
            e.Result = Task.Run<DevExpress.Xpf.Data.FetchRowsResult>(() =>
            {
                var context = new EFCoreIssues.Issues.IssuesContext();
                var queryable = context.Issues.AsNoTracking()
                    .SortBy(e.SortOrder, defaultUniqueSortPropertyName: nameof(EFCoreIssues.Issues.Issue.Id))
                    .Where(MakeFilterExpression(e.Filter));
                return queryable.Skip(e.Skip).Take(e.Take * pageTakeCount).ToArray();
            });
        }

        void OnGetTotalSummaries(System.Object sender, DevExpress.Xpf.Data.GetSummariesAsyncEventArgs e) {
            e.Result = Task.Run(() =>
            {
                var context = new EFCoreIssues.Issues.IssuesContext();
                var queryable = context.Issues.Where(MakeFilterExpression(e.Filter));
                return queryable.GetSummaries(e.Summaries);
            });
        }

        System.Linq.Expressions.Expression<System.Func<EFCoreIssues.Issues.Issue, bool>> MakeFilterExpression(DevExpress.Data.Filtering.CriteriaOperator filter) {
            var converter = new DevExpress.Xpf.Data.GridFilterCriteriaToExpressionConverter<EFCoreIssues.Issues.Issue>();
            return converter.Convert(filter);
        }

        void OnValidateRow(System.Object sender, DevExpress.Xpf.Grid.GridRowValidationEventArgs e) {
            var row = (EFCoreIssues.Issues.Issue)e.Row;
            var context = new EFCoreIssues.Issues.IssuesContext();
            context.Entry(row).State = e.IsNewItem
                ? EntityState.Added
                : EntityState.Modified;
            try {
                context.SaveChanges();
            } finally {
                context.Entry(row).State = EntityState.Detached;
            }
        }

        void LoadLookupData() {
            var context = new EFCoreIssues.Issues.IssuesContext();
            usersLookup.ItemsSource = context.Users.Select(user => new { Id = user.Id, Name = user.FirstName + " " + user.LastName }).ToArray();
        }

        void OnRefreshDataSource(System.Object sender, DevExpress.Xpf.Grid.RefreshDataSourceEventArgs e) {
            LoadLookupData();
        }
    }
}
