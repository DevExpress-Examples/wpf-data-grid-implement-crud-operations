using DevExpress.CRUD.ViewModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DevExpress.CRUD.View {
    public class GridControlDeleteRefreshBehavior : Behavior<TableView> {
        public ICommand<RowDeleteArgs> OnDeleteCommand {
            get { return (ICommand<RowDeleteArgs>)GetValue(OnDeleteCommandProperty); }
            set { SetValue(OnDeleteCommandProperty, value); }
        }
        public static readonly DependencyProperty OnDeleteCommandProperty =
            DependencyProperty.Register("OnDeleteCommand", typeof(ICommand<RowDeleteArgs>), typeof(GridControlDeleteRefreshBehavior), new PropertyMetadata(null));

        public ICommand<RefreshArgs> OnRefreshCommand {
            get { return (ICommand<RefreshArgs>)GetValue(OnRefreshCommandProperty); }
            set { SetValue(OnRefreshCommandProperty, value); }
        }
        public static readonly DependencyProperty OnRefreshCommandProperty =
            DependencyProperty.Register("OnRefreshCommand", typeof(ICommand<RefreshArgs>), typeof(GridControlDeleteRefreshBehavior), new PropertyMetadata(null));

        public string NoRecordsErrorMessage {
            get { return (string)GetValue(NoRecordsErrorMessageProperty); }
            set { SetValue(NoRecordsErrorMessageProperty, value); }
        }
        public static readonly DependencyProperty NoRecordsErrorMessageProperty =
            DependencyProperty.Register("NoRecordsErrorMessage", typeof(string), typeof(GridControlDeleteRefreshBehavior), new PropertyMetadata(null, (d, e) => { ((GridControlDeleteRefreshBehavior)d).UpdateErrorText(); }));


        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        TableView View => AssociatedObject;
        VirtualSourceBase VirtualSource => View?.DataControl?.ItemsSource as VirtualSourceBase;

        public GridControlDeleteRefreshBehavior() {
            DeleteCommand = new DelegateCommand(DoDelete, CanDelete);
            RefreshCommand = new AsyncCommand(DoRefresh, CanRefresh);
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

        async void OnPreviewKeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.Delete && CanDelete()) {
                e.Handled = true;
                DoDelete();
            }
            if(e.Key == Key.F5 && CanRefresh()) {
                e.Handled = true;
                await DoRefresh();
            }
        }

        bool isRefreshInProgress;
        async Task DoRefresh() {
            VirtualSource?.RefreshRows();
            var args = new RefreshArgs();
            OnRefreshCommand.Execute(args);
            if(args.Result != null) {
                isRefreshInProgress = true;
                try {
                    await args.Result;
                } finally {
                    isRefreshInProgress = false;
                }
            }
        }
        bool CanRefresh() {
            var canRefreshVirtualSource = VirtualSource == null
                || ((VirtualSource as InfiniteAsyncSource)?.IsResetting != true && !VirtualSource.AreRowsFetching);
            return canRefreshVirtualSource
                && OnRefreshCommand != null 
                && !IsEditingRowState() 
                && !isRefreshInProgress
                && (View?.Grid.ItemsSource != null || NoRecordsErrorMessage != null);
        }

        void DoDelete() {
            var row = View.Grid.SelectedItem;
            if(row == null)
                return;
            if(DXMessageBox.Show(View, "Are you sure you want to delete this row?", "Delete Row", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            try {
                OnDeleteCommand.Execute(new RowDeleteArgs(row));
                if(VirtualSource != null)
                    VirtualSource?.RefreshRows();
                else 
                    View.Commands.DeleteFocusedRow.Execute(null);
            } catch(Exception ex) {
                DXMessageBox.Show(ex.Message);
            }
        }

        bool CanDelete() {
            return OnDeleteCommand != null && !IsEditingRowState() && !isRefreshInProgress && View?.Grid.CurrentItem != null;
        }

        bool IsEditingRowState() {
            return View?.AreUpdateRowButtonsShown == true;
        }
    }
}
