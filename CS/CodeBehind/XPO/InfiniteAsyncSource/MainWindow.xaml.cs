using System.Windows;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace XPOIssues {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            using(var session = new Session()) {
                var classInfo = session.GetClassInfo<XPOIssues.Issues.Issue>();
                var properties = classInfo.Members
                    .Where(member => member.IsPublic && member.IsPersistent)
                    .Select(member => member.Name)
                    .ToArray();
                _DetachedObjectsHelper = DetachedObjectsHelper<XPOIssues.Issues.Issue>.Create(classInfo.KeyProperty.Name, properties);
            }
            var source = new InfiniteAsyncSource
            {
                CustomProperties = _DetachedObjectsHelper.Properties,
                KeyProperty = nameof(XPOIssues.Issues.Issue.Oid)
            };
            source.FetchRows += OnFetchRows;
            source.GetTotalSummaries += OnGetTotalSummaries;
            grid.ItemsSource = source;
            LoadLookupData();
        }
        DetachedObjectsHelper<XPOIssues.Issues.Issue> _DetachedObjectsHelper;

        void OnFetchRows(System.Object sender, DevExpress.Xpf.Data.FetchRowsAsyncEventArgs e) {
            e.Result = Task.Run<DevExpress.Xpf.Data.FetchRowsResult>(() =>
            {
                using(var session = new DevExpress.Xpo.Session()) {
                    var queryable = session.Query<XPOIssues.Issues.Issue>().SortBy(e.SortOrder, defaultUniqueSortPropertyName: nameof(XPOIssues.Issues.Issue.Oid)).Where(MakeFilterExpression(e.Filter));
                    var items = queryable.Skip(e.Skip).Take(e.Take ?? 100).ToArray();
                    return _DetachedObjectsHelper.ConvertToDetachedObjects(items);
                }
            });
        }

        void OnGetTotalSummaries(System.Object sender, DevExpress.Xpf.Data.GetSummariesAsyncEventArgs e) {
            e.Result = Task.Run(() =>
            {
                using(var session = new DevExpress.Xpo.Session()) {
                    return session.Query<XPOIssues.Issues.Issue>().Where(MakeFilterExpression(e.Filter)).GetSummaries(e.Summaries);
                }
            });
        }

        System.Linq.Expressions.Expression<System.Func<XPOIssues.Issues.Issue, bool>> MakeFilterExpression(DevExpress.Data.Filtering.CriteriaOperator filter) {
            var converter = new DevExpress.Xpf.Data.GridFilterCriteriaToExpressionConverter<XPOIssues.Issues.Issue>();
            return converter.Convert(filter);
        }

        void OnValidateRow(System.Object sender, DevExpress.Xpf.Grid.GridRowValidationEventArgs e) {
            using(var unitOfWork = new DevExpress.Xpo.UnitOfWork()) {
                var item = e.IsNewItem
                    ? new XPOIssues.Issues.Issue(unitOfWork)
                    : unitOfWork.GetObjectByKey<XPOIssues.Issues.Issue>(_DetachedObjectsHelper.GetKey(e.Row));
                _DetachedObjectsHelper.ApplyProperties(item, e.Row);
                unitOfWork.CommitChanges();
                if(e.IsNewItem) {
                    _DetachedObjectsHelper.SetKey(e.Row, item.Oid);
                }
            }
        }

        void OnValidateRowDeletion(System.Object sender, DevExpress.Xpf.Grid.GridValidateRowDeletionEventArgs e) {
            using(var unitOfWork = new DevExpress.Xpo.UnitOfWork()) {
                var key = _DetachedObjectsHelper.GetKey(e.Rows.Single());
                var item = unitOfWork.GetObjectByKey<XPOIssues.Issues.Issue>(key);
                unitOfWork.Delete(item);
                unitOfWork.CommitChanges();
            }
        }

        void LoadLookupData() {
            var session = new DevExpress.Xpo.Session();
            usersLookup.ItemsSource = session.Query<XPOIssues.Issues.User>().OrderBy(user => user.Oid).Select(user => new { Id = user.Oid, Name = user.FirstName + " " + user.LastName }).ToArray();
        }

        void OnDataSourceRefresh(System.Object sender, DevExpress.Xpf.Grid.DataSourceRefreshEventArgs e) {
            LoadLookupData();
        }
    }
}
