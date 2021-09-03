using DevExpress.CRUD.DataModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using System;
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
        public void OnDeleteRow(DeleteRowValidationArgs args) => OnDelete(args);

        void OnDelete(DeleteRowValidationArgs args) {
            try {
                //TODO: dont delete if data is in refresh state
                dataProvider.Delete((T)args.Item);
            } catch (Exception ex) {
                args.Result = ex.Message;
            }
        }

        [Command]
        public void OnRefresh(RefreshArgs _) {
            OnRefresh();
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

        [Command]
        public void OnUpdateRow(RowValidationArgs args) {
            var entity = (T)args.Item;
            if(args.IsNewItem)
                dataProvider.Create(entity);
            else
                dataProvider.Update(entity);
        }
    }
}
