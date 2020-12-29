using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DevExpress.CRUD.View {
    public class GridControlCRUDAsyncBehavior : Behavior<TableView> {
        public IAsyncCommand OnCreateCommand {
            get { return (IAsyncCommand)GetValue(OnCreateCommandProperty); }
            set { SetValue(OnCreateCommandProperty, value); }
        }
        public static readonly DependencyProperty OnCreateCommandProperty =
            DependencyProperty.Register("OnCreateCommand", typeof(IAsyncCommand), typeof(GridControlCRUDAsyncBehavior), new PropertyMetadata(null));

        public IAsyncCommand OnUpdateCommand {
            get { return (IAsyncCommand)GetValue(OnUpdateCommandProperty); }
            set { SetValue(OnUpdateCommandProperty, value); }
        }
        public static readonly DependencyProperty OnUpdateCommandProperty =
            DependencyProperty.Register("OnUpdateCommand", typeof(IAsyncCommand), typeof(GridControlCRUDAsyncBehavior), new PropertyMetadata(null));

        public ICommand OnDeleteCommand {
            get { return (ICommand)GetValue(OnDeleteCommandProperty); }
            set { SetValue(OnDeleteCommandProperty, value); }
        }
        public static readonly DependencyProperty OnDeleteCommandProperty =
            DependencyProperty.Register("OnDeleteCommand", typeof(ICommand), typeof(GridControlCRUDAsyncBehavior), new PropertyMetadata(null));

        public IAsyncCommand OnRefreshCommand {
            get { return (IAsyncCommand)GetValue(OnRefreshCommandProperty); }
            set { SetValue(OnRefreshCommandProperty, value); }
        }
        public static readonly DependencyProperty OnRefreshCommandProperty =
            DependencyProperty.Register("OnRefreshCommand", typeof(IAsyncCommand), typeof(GridControlCRUDAsyncBehavior), new PropertyMetadata(null));

        public string NoRecordsErrorMessage {
            get { return (string)GetValue(NoRecordsErrorMessageProperty); }
            set { SetValue(NoRecordsErrorMessageProperty, value); }
        }
        public static readonly DependencyProperty NoRecordsErrorMessageProperty =
            DependencyProperty.Register("NoRecordsErrorMessage", typeof(string), typeof(GridControlCRUDAsyncBehavior), new PropertyMetadata(null, (d, e) => { ((GridControlCRUDAsyncBehavior)d).UpdateErrorText(); }));


        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        TableView View => AssociatedObject;

        public GridControlCRUDAsyncBehavior() {
            DeleteCommand = new DelegateCommand(DoDelete, CanDelete);
            RefreshCommand = new AsyncCommand(DoRefresh, CanRefresh);
        }

        protected override void OnAttached() {
            base.OnAttached();
            View.ValidateRow += OnValidateRow;
            View.PreviewKeyDown += OnPreviewKeyDown;
            UpdateErrorText();
        }

        protected override void OnDetaching() {
            View.ValidateRow -= OnValidateRow;
            View.PreviewKeyDown -= OnPreviewKeyDown;
            UpdateErrorText();
            base.OnDetaching();
        }

        void UpdateErrorText() {
            if(View == null)
                return;
            if(NoRecordsErrorMessage != null) {
                View.ShowEmptyText = true;
                View.RuntimeLocalizationStrings = new GridRuntimeStringCollection() {
                    new RuntimeStringIdInfo(GridControlRuntimeStringId.NoRecords, NoRecordsErrorMessage)
                };
            } else {
                View.ShowEmptyText = false;
                View.RuntimeLocalizationStrings = null;
            }
        }

        async void OnPreviewKeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.Delete && CanDelete()) {
                DoDelete();
                e.Handled = true;
            }
            if(e.Key == Key.F5 && CanRefresh()) {
                await DoRefresh();
                e.Handled = true;
            }
        }

        Task DoRefresh() {
            return OnRefreshCommand.ExecuteAsync(null);
        }
        bool CanRefresh() {
            return OnRefreshCommand != null 
                && !IsEditingRowState() 
                && !OnRefreshCommand.IsExecuting 
                && (View?.Grid.ItemsSource != null || NoRecordsErrorMessage != null);
        }

        void DoDelete() {
            var row = View.Grid.SelectedItem;
            if(row == null)
                return;
            if(DXMessageBox.Show(View, "Are you sure you want to delete this row?", "Delete Row", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            try {
                OnDeleteCommand.Execute(row);
                View.Commands.DeleteFocusedRow.Execute(null);
            } catch(Exception ex) {
                DXMessageBox.Show(ex.Message);
            }
        }

        bool CanDelete() {
            return OnDeleteCommand != null && !IsEditingRowState() && !OnRefreshCommand.IsExecuting && View?.Grid.CurrentItem != null;
        }

        bool IsEditingRowState() {
            return View?.AreUpdateRowButtonsShown == true;
        }

        void OnValidateRow(object sender, GridRowValidationEventArgs e) {
            if(View.FocusedRowHandle == DataControlBase.NewItemRowHandle)
                e.UpdateRowResult = OnCreateCommand.ExecuteAsync(e.Row);
            else
                e.UpdateRowResult = OnUpdateCommand.ExecuteAsync(e.Row);
        }
    }
}
