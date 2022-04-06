using System.Windows;
using EntityFrameworkIssues.Issues;
using System.Data.Entity;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpf.Grid;

namespace EntityFrameworkIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            var source = new PagedAsyncSource
            {
                ElementType = typeof(Issue),
                KeyProperty = nameof(Issue.Id),
                PageNavigationMode = PageNavigationMode.ArbitraryWithTotalPageCount
            };
            source.FetchPage += OnFetchPage;
            source.GetTotalSummaries += OnGetTotalSummaries;
            grid.ItemsSource = source;
            LoadLookupData();
        }

        System.Linq.Expressions.Expression<System.Func<Issue, bool>> MakeFilterExpression(DevExpress.Data.Filtering.CriteriaOperator filter) {
            var converter = new DevExpress.Xpf.Data.GridFilterCriteriaToExpressionConverter<Issue>();
            return converter.Convert(filter);
        }
        void OnFetchPage(object sender, FetchPageAsyncEventArgs e) {
            const int pageTakeCount = 5;
            e.Result = Task.Run<DevExpress.Xpf.Data.FetchRowsResult>(() =>
            {
                var context = new IssuesContext();
                var queryable = context.Issues.AsNoTracking()
                    .SortBy(e.SortOrder, defaultUniqueSortPropertyName: nameof(Issue.Id))
                    .Where(MakeFilterExpression(e.Filter));
                return queryable.Skip(e.Skip).Take(e.Take * pageTakeCount).ToArray();
            });
        }
        void OnGetTotalSummaries(object sender, GetSummariesAsyncEventArgs e) {
            e.Result = Task.Run(() =>
            {
                var context = new IssuesContext();
                var queryable = context.Issues.Where(MakeFilterExpression(e.Filter));
                return queryable.GetSummaries(e.Summaries);
            });
        }
        void OnValidateRow(object sender, GridRowValidationEventArgs e) {
            var row = (Issue)e.Row;
            var context = new IssuesContext();
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
            var context = new EntityFrameworkIssues.Issues.IssuesContext();
            usersLookup.ItemsSource = context.Users.Select(user => new { Id = user.Id, Name = user.FirstName + " " + user.LastName }).ToArray();
        }
        void OnDataSourceRefresh(object sender, DevExpress.Xpf.Grid.DataSourceRefreshEventArgs e) {
            LoadLookupData();
        }
    }
}
