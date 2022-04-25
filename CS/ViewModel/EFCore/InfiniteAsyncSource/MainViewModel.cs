using DevExpress.Mvvm;
using EFCoreIssues.Issues;
using Microsoft.EntityFrameworkCore;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm.Xpf;

namespace EFCoreIssues {
    public class MainViewModel : ViewModelBase {

        System.Linq.Expressions.Expression<System.Func<Issue, bool>> MakeFilterExpression(DevExpress.Data.Filtering.CriteriaOperator filter) {
            var converter = new DevExpress.Xpf.Data.GridFilterCriteriaToExpressionConverter<Issue>();
            return converter.Convert(filter);
        }
        [Command]
        public void FetchRows(FetchRowsAsyncArgs args) {
            args.Result = Task.Run<DevExpress.Xpf.Data.FetchRowsResult>(() => {
                var context = new IssuesContext();
                var queryable = context.Issues.AsNoTracking()
                    .SortBy(args.SortOrder, defaultUniqueSortPropertyName: nameof(Issue.Id))
                    .Where(MakeFilterExpression((DevExpress.Data.Filtering.CriteriaOperator)args.Filter));
                return queryable.Skip(args.Skip).Take(args.Take ?? 100).ToArray();
            });
        }
        [Command]
        public void GetTotalSummaries(GetSummariesAsyncArgs args) {
            args.Result = Task.Run(() => {
                var context = new IssuesContext();
                var queryable = context.Issues.Where(MakeFilterExpression((DevExpress.Data.Filtering.CriteriaOperator)args.Filter));
                return queryable.GetSummaries(args.Summaries);
            });
        }
        [Command]
        public void ValidateRow(RowValidationArgs args) {
            var item = (Issue)args.Item;
            var context = new IssuesContext();
            context.Entry(item).State = args.IsNewItem ? EntityState.Added : EntityState.Modified;
            try {
                context.SaveChanges();
            } finally {
                context.Entry(item).State = EntityState.Detached;
            }
        }
        [Command]
        public void ValidateRowDeletion(ValidateRowDeletionArgs args) {
            var item = (Issue)args.Items.Single();
            var context = new IssuesContext();
            context.Entry(item).State = EntityState.Deleted;
            context.SaveChanges();
        }
        System.Collections.IList _Users;
        public System.Collections.IList Users {
            get
            {
                if(_Users == null && !DevExpress.Mvvm.ViewModelBase.IsInDesignMode) {
                    var context = new EFCoreIssues.Issues.IssuesContext();
                    _Users = context.Users.Select(user => new { Id = user.Id, Name = user.FirstName + " " + user.LastName }).ToArray();
                }
                return _Users;
            }
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void DataSourceRefresh(DevExpress.Mvvm.Xpf.DataSourceRefreshArgs args) {
            _Users = null;
            RaisePropertyChanged(nameof(Users));
        }
    }
}