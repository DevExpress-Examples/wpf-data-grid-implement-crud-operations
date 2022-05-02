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
            var source = new InfiniteAsyncSource {
                CustomProperties = _DetachedObjectsHelper.Properties,
                KeyProperty = nameof(Issue.Oid)
            };
            source.FetchRows += OnFetchRows;
            source.GetTotalSummaries += OnGetTotalSummaries;
            grid.ItemsSource = source;
            LoadLookupData();
        }

        Expression<Func<Issue, bool>> MakeFilterExpression(CriteriaOperator filter) {
            var converter = new GridFilterCriteriaToExpressionConverter<Issue>();
            return converter.Convert(filter);
        }
        DetachedObjectsHelper<Issue> _DetachedObjectsHelper;

        void OnFetchRows(object sender, FetchRowsAsyncEventArgs e) {
            e.Result = Task.Run<FetchRowsResult>(() => {
                using(var session = new Session()) {
                    var queryable = session.Query<Issue>().SortBy(e.SortOrder, defaultUniqueSortPropertyName: nameof(Issue.Oid)).Where(MakeFilterExpression(e.Filter));
                    var items = queryable.Skip(e.Skip).Take(e.Take ?? 100).ToArray();
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

        void OnValidateRowDeletion(object sender, GridValidateRowDeletionEventArgs e) {
            using(var unitOfWork = new UnitOfWork()) {
                var key = _DetachedObjectsHelper.GetKey(e.Rows.Single());
                var item = unitOfWork.GetObjectByKey<Issue>(key);
                unitOfWork.Delete(item);
                unitOfWork.CommitChanges();
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
