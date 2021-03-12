using DevExpress.CRUD.ViewModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace GridControlCRUDMVVMInfiniteAsyncSource {
    public class VirtualSourceEditFormBehavior : Behavior<TableView> {
        public ICommand OnUpdateCommand {
            get { return (ICommand)GetValue(OnUpdateCommandProperty); }
            set { SetValue(OnUpdateCommandProperty, value); }
        }
        public static readonly DependencyProperty OnUpdateCommandProperty =
            DependencyProperty.Register("OnUpdateCommand", typeof(ICommand), typeof(VirtualSourceEditFormBehavior), new PropertyMetadata(null));

        public ICommand OnCreateCommand {
            get { return (ICommand)GetValue(OnCreateCommandProperty); }
            set { SetValue(OnCreateCommandProperty, value); }
        }
        public static readonly DependencyProperty OnCreateCommandProperty =
            DependencyProperty.Register("OnCreateCommand", typeof(ICommand), typeof(VirtualSourceEditFormBehavior), new PropertyMetadata(null));

        VirtualSourceBase Source => (VirtualSourceBase)AssociatedObject?.DataControl?.ItemsSource;

        public ICommand CreateCommand { get; }
        public ICommand UpdateCommand { get; }
        public VirtualSourceEditFormBehavior() {
            CreateCommand = new DelegateCommand(DoCreate);
            UpdateCommand = new DelegateCommand(() => DoUpdate(), CanUpdate);
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
            if(e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) != 0) {
                DoCreate();
                e.Handled = true;
            }
        }

        void DoUpdate(object entity = null) {
            entity = entity ?? AssociatedObject.DataControl.CurrentItem;
            var args = new EntityUpdateArgs(entity);
            OnUpdateCommand.Execute(args);
            if(args.Updated)
                ReloadRow(GetKey(entity));
        }
        void ReloadRow(object key) {
            if(Source is InfiniteAsyncSource)
                ((InfiniteAsyncSource)Source).ReloadRows(key);
            else if(Source is PagedAsyncSource)
                ((PagedAsyncSource)Source).ReloadRows(key);
            else
                throw new InvalidOperationException();
        }

        bool CanUpdate() {
            return OnUpdateCommand != null && CanChangeCurrentItem();
        }

        bool CanChangeCurrentItem() {
            return AssociatedObject?.DataControl?.CurrentItem != null;
        }

        void DoCreate() {
            var args = new EntityCreateArgs();
            OnCreateCommand.Execute(args);
            if(args.Entity != null)
                Source.RefreshRows();
        }

        object GetKey<T>(T entity) {
            ITypedList typedList = Source;
            var keyProperty = typedList.GetItemProperties(null)[Source.KeyProperty];
            return keyProperty.GetValue(entity);
        }
    }
}
