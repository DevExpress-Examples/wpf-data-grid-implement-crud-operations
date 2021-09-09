using DevExpress.CRUD.DataModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Data;
using DevExpress.Mvvm.Xpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.ComponentModel;
using DevExpress.Xpf.Core;

namespace DevExpress.CRUD.ViewModel {
    public abstract class VirtualCollectionViewModel<T> : ViewModelBase where T : class, new() {
        readonly IDataProvider<T> dataProvider;

        IMessageBoxService MessageBoxService => GetService<IMessageBoxService>();

        protected VirtualCollectionViewModel(IDataProvider<T> dataProvider) {
            this.dataProvider = dataProvider;
            StartRefresh();
        }

        public Type FilterType => typeof(Expression<Func<T, bool>>);
        IDialogService DialogService => GetService<IDialogService>();

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
        public void OnUpdate(EntityUpdateArgs args) {
            var entity = (T)args.Entity;
            var commands = CreateCommands(() => {
                dataProvider.Update(entity);
                args.Updated = true;
            });
            DialogService.ShowDialog(commands, "Edit " + typeof(T).Name, CreateEntityViewModel(entity));
        }

        [Command]
        public void OnCreate(EntityCreateArgs args) {
            var entity = new T();
            var commands = CreateCommands(() => {
                dataProvider.Create(entity);
                args.Entity = entity;
            });
            DialogService.ShowDialog(commands, "New " + typeof(T).Name, CreateEntityViewModel(entity));
        }

        UICommand[] CreateCommands(Action saveAction) {
            return new[] {
                new UICommand(null, "Save", new DelegateCommand<CancelEventArgs>(cancelArgs => {
                    try {
                        saveAction();
                    } catch(Exception e) {
                        GetService<IMessageBoxService>().ShowMessage(e.Message, "Error", MessageButton.OK);
                        cancelArgs.Cancel = true;
                    }
                }), isDefault: true, isCancel: false),
                new UICommand(null, "Cancel", null, isDefault: false, isCancel: true),
            };
        }

        protected abstract EntityViewModel<T> CreateEntityViewModel(T entity);

        [Command]
        public void OnDeleteRow(DeleteRowValidationArgs args) => OnDelete(args);

        void OnDelete(DeleteRowValidationArgs args) {
            if(MessageBoxService.ShowMessage("Are you sure you want to delete this row?", "Delete Row", MessageButton.OKCancel) == MessageResult.Cancel) {
                args.Result = "Not accepted";
                return;
            }
            try {
                dataProvider.Delete((T)args.Items[0]);
            } catch(Exception ex) {
                MessageBoxService.ShowMessage(ex.Message);
                args.Result = ex.Message;
            }
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
