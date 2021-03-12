using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using System;
using System.Windows;
using System.Windows.Input;

namespace DevExpress.CRUD.View {
    public class GridControlCRUDBehavior : Behavior<TableView> {
        public ICommand OnDeleteCommand {
            get { return (ICommand)GetValue(OnDeleteCommandProperty); }
            set { SetValue(OnDeleteCommandProperty, value); }
        }
        public static readonly DependencyProperty OnDeleteCommandProperty =
            DependencyProperty.Register("OnDeleteCommand", typeof(ICommand), typeof(GridControlCRUDBehavior), new PropertyMetadata(null));

        public ICommand OnRefreshCommand {
            get { return (ICommand)GetValue(OnRefreshCommandProperty); }
            set { SetValue(OnRefreshCommandProperty, value); }
        }
        public static readonly DependencyProperty OnRefreshCommandProperty =
            DependencyProperty.Register("OnRefreshCommand", typeof(ICommand), typeof(GridControlCRUDBehavior), new PropertyMetadata(null));

        public string NoRecordsErrorMessage {
            get { return (string)GetValue(NoRecordsErrorMessageProperty); }
            set { SetValue(NoRecordsErrorMessageProperty, value); }
        }
        public static readonly DependencyProperty NoRecordsErrorMessageProperty =
            DependencyProperty.Register("NoRecordsErrorMessage", typeof(string), typeof(GridControlCRUDBehavior), new PropertyMetadata(null, (d, e) => { ((GridControlCRUDBehavior)d).UpdateErrorText(); }));

        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        TableView View => AssociatedObject;

        public GridControlCRUDBehavior() {
            DeleteCommand = new DelegateCommand(DoDelete, CanDelete);
            RefreshCommand = new DelegateCommand(DoRefresh, CanRefresh);
        }

        protected override void OnAttached() {
            base.OnAttached();
            View.PreviewKeyDown += OnPreviewKeyDown;
            UpdateErrorText();
        }

        protected override void OnDetaching() {
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

        void OnPreviewKeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.Delete && CanDelete()) {
                DoDelete();
                e.Handled = true;
            }
            if(e.Key == Key.F5 && CanRefresh()) {
                DoRefresh();
                e.Handled = true;
            }
        }

        void DoRefresh() {
            OnRefreshCommand.Execute(null);
        }
        bool CanRefresh() {
            return OnRefreshCommand != null && !IsEditingRowState();
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
            return OnDeleteCommand != null && !IsEditingRowState();
        }

        bool IsEditingRowState() {
            return View?.AreUpdateRowButtonsShown == true;
        }
    }
}
