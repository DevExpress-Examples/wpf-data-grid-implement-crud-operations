using DevExpress.CRUD.DataModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using System.Collections.Generic;

namespace DevExpress.CRUD.ViewModel {
    public abstract class CollectionViewModel<T> : ViewModelBase where T : class {
        readonly IDataProvider<T> dataProvider;

        protected CollectionViewModel(IDataProvider<T> dataProvider) {
            this.dataProvider = dataProvider;
            OnRefresh();
        }

        public IList<T> Entities {
            get => GetValue<IList<T>>();
            private set => SetValue(value);
        }
        public string EntitiesErrorMessage {
            get => GetValue<string>();
            private set => SetValue(value);
        }

        [Command]
        public void OnRefresh() {
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

        [Command]
        public void OnCreate(T entity) => dataProvider.Create(entity);

        [Command]
        public void OnUpdate(T entity) => dataProvider.Update(entity);

        [Command]
        public void OnDelete(T entity) => dataProvider.Delete(entity);
    }
}
