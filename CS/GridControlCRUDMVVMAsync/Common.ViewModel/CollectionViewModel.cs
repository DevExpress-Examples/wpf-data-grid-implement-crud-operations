using DevExpress.CRUD.DataModel;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevExpress.CRUD.ViewModel {
    public abstract class CollectionViewModel<T> : ViewModelBase where T : class {
        readonly IDataProvider<T> dataProvider;

        protected CollectionViewModel(IDataProvider<T> dataProvider) {
            this.dataProvider = dataProvider;
            StartRefresh();
            OnRefreshCommand = new AsyncCommand(OnRefreshAsync);
            OnCreateCommand = new AsyncCommand<T>(entity => this.dataProvider.CreateAsync(entity));
            OnUpdateCommand = new AsyncCommand<T>(entity => this.dataProvider.UpdateAsync(entity));
            OnDeleteCommand = new DelegateCommand<T>(this.dataProvider.Delete);
        }

        public AsyncCommand OnRefreshCommand { get; }
        public AsyncCommand<T> OnCreateCommand { get; }
        public AsyncCommand<T> OnUpdateCommand { get; }
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
