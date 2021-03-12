using DevExpress.CRUD.DataModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevExpress.CRUD.ViewModel {
    public abstract class CollectionViewModel<T> : ViewModelBase where T : class {
        readonly IDataProvider<T> dataProvider;

        protected CollectionViewModel(IDataProvider<T> dataProvider) {
            this.dataProvider = dataProvider;
            StartRefresh();
        }

        public IList<T> Entities {
            get => GetValue<IList<T>>();
            private set => SetValue(value);
        }
        public string EntitiesErrorMessage {
            get => GetValue<string>();
            private set => SetValue(value);
        }
        public bool IsLoading {
            get => GetValue<bool>();
            private set => SetValue(value);
        }

        async void StartRefresh() {
            await OnRefreshAsync();
        }

        [Command]
        public void OnRefresh(RefreshArgs args) {
            args.Result = OnRefreshAsync();
        }
        async Task OnRefreshAsync() {
            IsLoading = true;
            try {
                await Task.WhenAll(RefreshEntities(), OnRefreshCoreAsync());
            } finally {
                IsLoading = false;
            }
        }
        async Task RefreshEntities() {
            try {
                Entities = await dataProvider.ReadAsync();
                EntitiesErrorMessage = null;
            } catch {
                Entities = null;
                EntitiesErrorMessage = "An error has occurred while establishing a connection to the database. Press F5 to retry the connection.";
            }
        }
        protected virtual Task OnRefreshCoreAsync() {
            return Task.CompletedTask;
        }

        [Command]
        public void OnUpdateRow(RowValidationArgs args) {
            args.ResultAsync = OnUpdateRowAsync((T)args.Row, args.IsNewRow);
        }
        async Task<ValidationErrorInfo> OnUpdateRowAsync(T entity, bool isNew) {
            if(isNew)
                await dataProvider.CreateAsync(entity);
            else
                await dataProvider.UpdateAsync(entity);
            return null;
        }

        [Command]
        public void OnDelete(RowDeleteArgs args) => dataProvider.Delete((T)args.Row);
    }
}
