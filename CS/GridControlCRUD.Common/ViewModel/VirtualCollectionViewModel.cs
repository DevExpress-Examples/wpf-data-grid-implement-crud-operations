using DevExpress.CRUD.DataModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Data;
using DevExpress.Mvvm.Xpf;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExpress.CRUD.ViewModel {
    public abstract class VirtualCollectionViewModel<T> : ViewModelBase where T : class, new() {
        readonly IDataProvider<T> dataProvider;

        public IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }

        protected VirtualCollectionViewModel(IDataProvider<T> dataProvider) {
            this.dataProvider = dataProvider;
            StartRefresh();
        }

        public Type FilterType => typeof(Expression<Func<T, bool>>);

        [Command]
        public void Fetch(FetchRowsAsyncArgs args) {
            args.Result = dataProvider.GetQueryableResultAsync<T, FetchRowsResult>(queryable => {
                return queryable
                    .SortBy(args.SortOrder, defaultUniqueSortPropertyName: dataProvider.KeyProperty)
                    .Where((Expression<Func<T, bool>>)args.Filter)
                    .Skip(args.Skip)
                    .Take(args.Take ?? 30)
                    .ToArray<object>();
            });
        }

        [Command]
        public void GetTotalSummaries(GetSummariesAsyncArgs args) {
            args.Result = dataProvider.GetQueryableResultAsync(queryable => {
                return queryable
                    .Where((Expression<Func<T, bool>>)args.Filter)
                    .GetSummaries(args.Summaries);
            });
        }

        [Command]
        public void GetUniqueValues(GetUniqueValuesAsyncArgs args) {
            args.ResultWithCounts = dataProvider.GetQueryableResultAsync(queryable => {
                return queryable
                    .Where((Expression<Func<T, bool>>)args.Filter)
                    .DistinctWithCounts(args.PropertyName);
            });
        }

        [Command]
        public void OnCreateRow(RowValidationArgs args) {
            if(args.IsNewItem) {
                dataProvider.Create((T)args.Item);
            } else {
                dataProvider.Update((T)args.Item);
            }
        }

        [Command]
        public void OnDeleteRow(DeleteRowsValidationArgs args) {
            var row = (T)args.Items[0];
            if(row == null)
                return;
            if(MessageBoxService.ShowMessage("Are you sure you want to delete this row?", "Delete Row", MessageButton.OKCancel) == MessageResult.Cancel) {
                args.Result = "Canceled";
            }
            this.dataProvider.Delete(row);
        }

        [Command]
        public void OnRefresh(RefreshArgs args) {
            args.Result = OnRefreshCoreAsync();
        }
        async void StartRefresh() {
            await OnRefreshCoreAsync();
        }
        protected virtual Task OnRefreshCoreAsync() {
            return Task.CompletedTask;
        }
    }
    public class EntityViewModel<T> : ViewModelBase {
        public EntityViewModel(T entity) {
            Entity = entity;
        }
        public T Entity { get; }
    }
}
