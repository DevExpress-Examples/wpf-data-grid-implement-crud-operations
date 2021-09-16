﻿using DevExpress.CRUD.DataModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevExpress.CRUD.ViewModel {
    public abstract class AsyncCollectionViewModel<T> : ViewModelBase where T : class {
        readonly IDataProvider<T> dataProvider;

        public IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }

        protected AsyncCollectionViewModel(IDataProvider<T> dataProvider) {
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
            args.ResultAsync = OnUpdateRowAsync((T)args.Item, args.IsNewItem);
        }
        async Task<ValidationErrorInfo> OnUpdateRowAsync(T entity, bool isNew) {
            if(isNew)
                await dataProvider.CreateAsync(entity);
            else
                await dataProvider.UpdateAsync(entity);
            return null;
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
