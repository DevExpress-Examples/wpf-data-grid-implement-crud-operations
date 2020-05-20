using DevExpress.CRUD.DataModel;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Windows.Input;

namespace DevExpress.CRUD.ViewModel {
    public abstract class CollectionViewModel<T> : ViewModelBase where T : class {
        readonly ICRUDDataProvider<T> dataProvider;

        protected CollectionViewModel(ICRUDDataProvider<T> dataProvider) {
            this.dataProvider = dataProvider;
            OnRefresh();
            OnRefreshCommand = new DelegateCommand(OnRefresh);
            OnCreateCommand = new DelegateCommand<T>(this.dataProvider.Create);
            OnUpdateCommand = new DelegateCommand<T>(this.dataProvider.Update);
            OnDeleteCommand = new DelegateCommand<T>(this.dataProvider.Delete);
        }

        public ICommand OnRefreshCommand { get; }
        public ICommand<T> OnCreateCommand { get; }
        public ICommand<T> OnUpdateCommand { get; }
        public ICommand<T> OnDeleteCommand { get; }

        public IList<T> Entities {
            get => GetValue<IList<T>>();
            private set => SetValue(value);
        }
        public string EntitiesErrorMessage {
            get => GetValue<string>();
            private set => SetValue(value);
        }

        void OnRefresh() {
            try {
                Entities = dataProvider.Read();
                EntitiesErrorMessage = null;
            } catch {
                Entities = null;
                EntitiesErrorMessage = "An error has occurred while establishing a connection to the database. Press F5 to retry the connection.";
            }
            OnRefreshCore();
        }
        protected virtual void OnRefreshCore() {
        }
    }
}
