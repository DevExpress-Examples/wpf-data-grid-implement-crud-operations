using DevExpress.CRUD.ViewModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GridControlCRUDMVVMInfiniteAsyncSource {
    public class CRUDInfiniteAsyncSourceBehavior : Behavior<GridControl> {
        public ICommand OnUpdateCommand {
            get { return (ICommand)GetValue(OnUpdateCommandProperty); }
            set { SetValue(OnUpdateCommandProperty, value); }
        }
        public static readonly DependencyProperty OnUpdateCommandProperty =
            DependencyProperty.Register("OnUpdateCommand", typeof(ICommand), typeof(CRUDInfiniteAsyncSourceBehavior), new PropertyMetadata(null));

        public ICommand OnCreateCommand {
            get { return (ICommand)GetValue(OnCreateCommandProperty); }
            set { SetValue(OnCreateCommandProperty, value); }
        }
        public static readonly DependencyProperty OnCreateCommandProperty =
            DependencyProperty.Register("OnCreateCommand", typeof(ICommand), typeof(CRUDInfiniteAsyncSourceBehavior), new PropertyMetadata(null));

        public ICommand OnDeleteCommand {
            get { return (ICommand)GetValue(OnDeleteCommandProperty); }
            set { SetValue(OnDeleteCommandProperty, value); }
        }
        public static readonly DependencyProperty OnDeleteCommandProperty =
            DependencyProperty.Register("OnDeleteCommand", typeof(ICommand), typeof(CRUDInfiniteAsyncSourceBehavior), new PropertyMetadata(null));

        public IAsyncCommand OnRefreshCommand {
            get { return (IAsyncCommand)GetValue(OnRefreshCommandProperty); }
            set { SetValue(OnRefreshCommandProperty, value); }
        }
        public static readonly DependencyProperty OnRefreshCommandProperty =
            DependencyProperty.Register("OnRefreshCommand", typeof(IAsyncCommand), typeof(CRUDInfiniteAsyncSourceBehavior), new PropertyMetadata(null));

        InfiniteAsyncSource Source => (InfiniteAsyncSource)AssociatedObject.ItemsSource;

        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand UpdateCommand { get; }
        public CRUDInfiniteAsyncSourceBehavior() {
            DeleteCommand = new DelegateCommand(DoDelete, CanDelete);
            CreateCommand = new DelegateCommand(DoCreate);
            UpdateCommand = new DelegateCommand(() => DoUpdate());
            RefreshCommand = new AsyncCommand(DoRefresh, CanRefresh);
        }

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.PreviewKeyDown += OnKeyDown;
            AssociatedObject.MouseDoubleClick += OnMouseDoubleClick;
        }

        protected override void OnDetaching() {
            AssociatedObject.PreviewKeyDown -= OnKeyDown;
            AssociatedObject.MouseDoubleClick -= OnMouseDoubleClick;
            base.OnDetaching();
        }

        void OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var row = EventArgsToDataRowConverter.GetDataRow(e);
            if(row != null)
                DoUpdate(row);
        }

        void OnKeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.F2) {
                DoUpdate();
                e.Handled = true;
            }
            if(e.Key == Key.Delete && CanDelete()) {
                DoDelete();
                e.Handled = true;
            }
            if(e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) != 0) {
                DoCreate();
                e.Handled = true;
            }
        }

        void DoUpdate(object entity = null) {
            typeof(CRUDInfiniteAsyncSourceBehavior)
                .GetMethod(nameof(DoUpdateCore), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(Source.ElementType)
                .Invoke(this, new object[] { entity ?? AssociatedObject.CurrentItem });
        }

        void DoUpdateCore<T>(T entity) {
            var args = new EntityUpdateArgs<T>(entity);
            OnUpdateCommand.Execute(args);
            if(args.Updated)
                Source.ReloadRows(GetKey(entity));
        }

        void DoCreate() {
            typeof(CRUDInfiniteAsyncSourceBehavior)
                .GetMethod(nameof(DoCreateCore), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(Source.ElementType)
                .Invoke(this, new object[] { });
        }

        void DoCreateCore<T>() {
            var args = new EntityCreateArgs<T>();
            OnCreateCommand.Execute(args);
            if(args.Entity != null)
                Source.RefreshRows();
        }

        object GetKey<T>(T entity) {
            return TypeDescriptor.GetProperties(typeof(T))[Source.KeyProperty].GetValue(entity);
        }

        void DoDelete() {
            var row = AssociatedObject.SelectedItem;
            if(row == null)
                return;
            if(DXMessageBox.Show(AssociatedObject, "Are you sure you want to delete this row?", "Delete Row", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            try {
                OnDeleteCommand.Execute(row);
                Source.RefreshRows();
            } catch(Exception ex) {
                DXMessageBox.Show(ex.Message);
            }
        }

        bool CanDelete() {
            return OnDeleteCommand != null && !OnRefreshCommand.IsExecuting && AssociatedObject.CurrentItem != null;
        }

        async Task DoRefresh() {
            Source.RefreshRows();
            await OnRefreshCommand.ExecuteAsync(null);
        }
        bool CanRefresh() {
            return !Source.IsResetting 
                && !Source.AreRowsFetching 
                && OnRefreshCommand != null
                && !OnRefreshCommand.IsExecuting
                && AssociatedObject.ItemsSource != null;
        }
    }
}
