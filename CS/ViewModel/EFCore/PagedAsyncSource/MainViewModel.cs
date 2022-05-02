using DevExpress.Mvvm;
using EFCoreIssues.Issues;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using System;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace EFCoreIssues {
    public class MainViewModel : ViewModelBase {

        Expression<Func<Issue, bool>> MakeFilterExpression(CriteriaOperator filter) {
            var converter = new GridFilterCriteriaToExpressionConverter<Issue>();
            return converter.Convert(filter);
        }
        [Command]
        public void FetchPage(FetchPageAsyncArgs args) {
            const int pageTakeCount = 5;
            args.Result = Task.Run<FetchRowsResult>(() => {
                var context = new IssuesContext();
                var queryable = context.Issues.AsNoTracking()
                    .SortBy(args.SortOrder, defaultUniqueSortPropertyName: nameof(Issue.Id))
                    .Where(MakeFilterExpression((CriteriaOperator)args.Filter));
                return queryable.Skip(args.Skip).Take(args.Take * pageTakeCount).ToArray();
            });
        }
        [Command]
        public void GetTotalSummaries(GetSummariesAsyncArgs args) {
            args.Result = Task.Run(() => {
                var context = new IssuesContext();
                var queryable = context.Issues.Where(MakeFilterExpression((CriteriaOperator)args.Filter));
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
        IList _Users;
        public IList Users {
            get
            {
                if(_Users == null && !DevExpress.Mvvm.ViewModelBase.IsInDesignMode) {
                    var context = new IssuesContext();
                    _Users = context.Users.Select(user => new { Id = user.Id, Name = user.FirstName + " " + user.LastName }).ToArray();
                }
                return _Users;
            }
        }
        [Command]
        public void DataSourceRefresh(DataSourceRefreshArgs args) {
            _Users = null;
            RaisePropertyChanged(nameof(Users));
        }
    }
}