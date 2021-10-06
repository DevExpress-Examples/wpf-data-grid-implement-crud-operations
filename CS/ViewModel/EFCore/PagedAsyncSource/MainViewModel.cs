using DevExpress.Mvvm;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFCoreIssues {
    public class MainViewModel : ViewModelBase {
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void FetchPage(DevExpress.Mvvm.Xpf.FetchPageAsyncArgs args) {
            const int pageTakeCount = 5;
            args.Result = Task.Run<DevExpress.Xpf.Data.FetchRowsResult>(() =>
            {
                var context = new EFCoreIssues.Issues.IssuesContext();
                var queryable = context.Issues.AsNoTracking()
                    .SortBy(args.SortOrder, defaultUniqueSortPropertyName: nameof(EFCoreIssues.Issues.Issue.Id))
                    .Where(MakeFilterExpression((DevExpress.Data.Filtering.CriteriaOperator)args.Filter));
                return queryable.Skip(args.Skip).Take(args.Take * pageTakeCount).ToArray();
            });
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void GetTotalSummaries(DevExpress.Mvvm.Xpf.GetSummariesAsyncArgs args) {
            args.Result = Task.Run(() =>
            {
                var context = new EFCoreIssues.Issues.IssuesContext();
                var queryable = context.Issues.Where(MakeFilterExpression((DevExpress.Data.Filtering.CriteriaOperator)args.Filter));
                return queryable.GetSummaries(args.Summaries);
            });
        }

        System.Linq.Expressions.Expression<System.Func<EFCoreIssues.Issues.Issue, bool>> MakeFilterExpression(DevExpress.Data.Filtering.CriteriaOperator filter) {
            var converter = new DevExpress.Xpf.Data.GridFilterCriteriaToExpressionConverter<EFCoreIssues.Issues.Issue>();
            return converter.Convert(filter);
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRow(DevExpress.Mvvm.Xpf.RowValidationArgs args) {
            var item = (EFCoreIssues.Issues.Issue)args.Item;
            var context = new EFCoreIssues.Issues.IssuesContext();
            context.Entry(item).State = args.IsNewItem ? EntityState.Added : EntityState.Modified;
            try {
                context.SaveChanges();
            } finally {
                context.Entry(item).State = EntityState.Detached;
            }
        }
        System.Collections.IList _Users;

        public System.Collections.IList Users
        {
            get
            {
                if(_Users == null && !IsInDesignMode) {
                    var context = new EFCoreIssues.Issues.IssuesContext();
                    _Users = context.Users.Select(user => new { Id = user.Id, Name = user.FirstName + " " + user.LastName }).ToArray();
                }
                return _Users;
            }
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void RefreshDataSource(DevExpress.Mvvm.Xpf.RefreshDataSourceArgs args) {
            _Users = null;
            RaisePropertyChanged(nameof(Users));
        }
    }
}