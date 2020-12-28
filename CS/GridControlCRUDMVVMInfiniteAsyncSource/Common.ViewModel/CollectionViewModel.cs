using DevExpress.CRUD.DataModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Data.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExpress.CRUD.ViewModel {
    public abstract class CollectionViewModel<T> : ViewModelBase where T : class, new() {
        readonly IDataProvider<T> dataProvider;

        protected CollectionViewModel(IDataProvider<T> dataProvider) {
            this.dataProvider = dataProvider;
            StartRefresh();
        }

        public Type FilterType => typeof(Expression<Func<T, bool>>);
        IDialogService DialogService => GetService<IDialogService>();

        [Command]
        public void Fetch(FetchRowsAsyncArgs<Expression<Func<T, bool>>> args) {
            args.Result = dataProvider.GetQueryableResultAsync<T, FetchRowsResult>(queryable => {
                return queryable
                    .SortBy(args.SortOrder, defaultUniqueSortPropertyName: dataProvider.KeyProperty)
                    .Where(args.Filter)
                    .Skip(args.Skip)
                    .Take(args.Take ?? 30)
                    .ToArray<object>();
            });
        }

        [Command]
        public void GetTotalSummaries(GetSummariesAsyncArgs<Expression<Func<T, bool>>> args) {
            args.Result = dataProvider.GetQueryableResultAsync(queryable => {
                return queryable
                    .Where(args.Filter)
                    .GetSummaries(args.Summaries);
            });
        }

        [Command]
        public void GetUniqueValues(GetUniqueValuesAsyncArgs<Expression<Func<T, bool>>> args) {
            args.ResultWithCounts = dataProvider.GetQueryableResultAsync(queryable => {
                return queryable
                    .Where(args.Filter)
                    .DistinctWithCounts(args.PropertyName);
            });
        }

        [Command]
        public void OnUpdate(EntityUpdateArgs<T> args) {
            var commands = CreateCommands(() => {
                dataProvider.Update(args.Entity);
                args.Updated = true;
            });
            DialogService.ShowDialog(commands, "Edit " + typeof(T).Name, CreateEntityViewModel(args.Entity));
        }

        [Command]
        public void OnCreate(EntityCreateArgs<T> args) {
            var entity = new T();
            var commands = CreateCommands(() => {
                dataProvider.Create(entity);
                args.Entity = entity;
            });
            DialogService.ShowDialog(commands, "New " + typeof(T).Name, CreateEntityViewModel(entity));
        }

        UICommand[] CreateCommands(Action saveAction) {
            return new[] {
                new UICommand(null, "Save", new DelegateCommand(saveAction), isDefault: true, isCancel: false),
                new UICommand(null, "Cancel", null, isDefault: false, isCancel: true),
            };
        }

        protected abstract EntityViewModel<T> CreateEntityViewModel(T entity);

        [Command]
        public void OnDelete(T entity) {
            this.dataProvider.Delete(entity);
        }

        [AsyncCommand]
        public Task OnRefresh() {
            return OnRefreshCoreAsync();
        }

        async void StartRefresh() {
            await OnRefresh();
        }

        protected virtual Task OnRefreshCoreAsync() {
            return Task.CompletedTask;
        }
    }
    public class EntityUpdateArgs<T> {
        public EntityUpdateArgs(T entity) {
            Entity = entity;
        }
        public T Entity { get; }
        public bool Updated { get; set; }
    }
    public class EntityCreateArgs<T> {
        public T Entity { get; set; }
    }
    public class EntityViewModel<T> : ViewModelBase {
        public EntityViewModel(T entity) {
            Entity = entity;
        }
        public T Entity { get; }
    }
}
