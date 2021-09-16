using DevExpress.CRUD.DataModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using System.Collections.Generic;

namespace DevExpress.CRUD.ViewModel {
    public abstract class CollectionViewModel<T> : ViewModelBase where T : class {
        readonly IDataProvider<T> dataProvider;

        IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }

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

        [Command]
        public void OnDeleteRow(DeleteRowsValidationArgs args) {
            var row = (T)args.Items[0];
            if(row == null)
                return;
            if(MessageBoxService.ShowMessage("Are you sure you want to delete this row?", "Delete Row", MessageButton.OKCancel) == MessageResult.Cancel) {
                args.Result = "Canceled";
            }
            dataProvider.Delete(row);
        }
    }
}
