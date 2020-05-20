using DevExpress.CRUD.DataModel;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevExpress.CRUD.ViewModel {
    public abstract class CollectionViewModel<T> : ViewModelBase where T : class {
        readonly ICRUDDataProvider<T> dataProvider;

        protected CollectionViewModel(ICRUDDataProvider<T> dataProvider) {
            this.dataProvider = dataProvider;
            StartRefresh();
            OnRefreshCommand = new AsyncCommand(OnRefreshAsync);
            OnCreateCommand = new AsyncCommand<object>(entity => this.dataProvider.CreateAsync((T)entity));
            OnUpdateCommand = new AsyncCommand<object>(entity => this.dataProvider.UpdateAsync((T)entity));
            OnDeleteCommand = new DelegateCommand<T>(this.dataProvider.Delete);
        }

        public AsyncCommand OnRefreshCommand { get; }
        public AsyncCommand<object> OnCreateCommand { get; }
        public AsyncCommand<object> OnUpdateCommand { get; }
        public ICommand<T> OnDeleteCommand { get; }

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
    }
}
