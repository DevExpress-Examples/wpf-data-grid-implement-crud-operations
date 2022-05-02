using System.Windows;
using XPOIssues.Issues;
using System;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.Xpf.Grid;

namespace XPOIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            using(var session = new Session()) {
                var classInfo = session.GetClassInfo<Issue>();
                var properties = classInfo.Members
                    .Where(member => member.IsPublic && member.IsPersistent)
                    .Select(member => member.Name)
                    .ToArray();
                _DetachedObjectsHelper = DetachedObjectsHelper<Issue>.Create(classInfo.KeyProperty.Name, properties);
            }
            var source = new PagedAsyncSource {
                CustomProperties = _DetachedObjectsHelper.Properties,
                KeyProperty = nameof(Issue.Oid),
                PageNavigationMode = PageNavigationMode.ArbitraryWithTotalPageCount
            };
            source.FetchPage += OnFetchPage;
            source.GetTotalSummaries += OnGetTotalSummaries;
            grid.ItemsSource = source;
            LoadLookupData();
        }

        Expression<Func<Issue, bool>> MakeFilterExpression(CriteriaOperator filter) {
            var converter = new GridFilterCriteriaToExpressionConverter<Issue>();
            return converter.Convert(filter);
        }
        DetachedObjectsHelper<Issue> _DetachedObjectsHelper;

        void OnFetchPage(object sender, FetchPageAsyncEventArgs e) {
            e.Result = Task.Run<FetchRowsResult>(() => {
                const int pageTakeCount = 5;
                using(var session = new Session()) {
                    var queryable = session.Query<Issue>().SortBy(e.SortOrder, defaultUniqueSortPropertyName: nameof(Issue.Oid)).Where(MakeFilterExpression((CriteriaOperator)e.Filter));
                    var items = queryable.Skip(e.Skip).Take(e.Take * pageTakeCount).ToArray();
                    return _DetachedObjectsHelper.ConvertToDetachedObjects(items);
                }
            });
        }

        void OnGetTotalSummaries(object sender, GetSummariesAsyncEventArgs e) {
            e.Result = Task.Run(() => {
                using(var session = new Session()) {
                    return session.Query<Issue>().Where(MakeFilterExpression(e.Filter)).GetSummaries(e.Summaries);
                }
            });
        }

        void OnValidateRow(object sender, GridRowValidationEventArgs e) {
            using(var unitOfWork = new UnitOfWork()) {
                var item = e.IsNewItem
                    ? new Issue(unitOfWork)
                    : unitOfWork.GetObjectByKey<Issue>(_DetachedObjectsHelper.GetKey(e.Row));
                _DetachedObjectsHelper.ApplyProperties(item, e.Row);
                unitOfWork.CommitChanges();
                if(e.IsNewItem) {
                    _DetachedObjectsHelper.SetKey(e.Row, item.Oid);
                }
            }
        }

        void LoadLookupData() {
            var session = new Session();
            usersLookup.ItemsSource = session.Query<User>().OrderBy(user => user.Oid).Select(user => new { Id = user.Oid, Name = user.FirstName + " " + user.LastName }).ToArray();
        }

        void OnDataSourceRefresh(object sender, DataSourceRefreshEventArgs e) {
            LoadLookupData();
        }
    }
}
