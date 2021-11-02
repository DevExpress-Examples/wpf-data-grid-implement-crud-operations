using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EFCoreIssues {
    public class UndoBehavior : Behavior<TableView> {
        public static readonly DependencyProperty CopyOperationsSupporterProperty =
           DependencyProperty.Register(nameof(CopyOperationsSupporter), typeof(IDataItemCopyOperationsSupporter), typeof(UndoBehavior), new PropertyMetadata(null));

        public IDataItemCopyOperationsSupporter CopyOperationsSupporter {
            get { return (IDataItemCopyOperationsSupporter)GetValue(CopyOperationsSupporterProperty); }
            set { SetValue(CopyOperationsSupporterProperty, value); }
        }

        public static readonly DependencyProperty ValidateRowCommandProperty =
          DependencyProperty.Register(nameof(ValidateRowCommand), typeof(ICommand<RowValidationArgs>), typeof(UndoBehavior), new PropertyMetadata(null));

        public ICommand<RowValidationArgs> ValidateRowCommand {
            get { return (ICommand<RowValidationArgs>)GetValue(ValidateRowCommandProperty); }
            set { SetValue(ValidateRowCommandProperty, value); }
        }

        public static readonly DependencyProperty ValidateRowDeletionCommandProperty =
        DependencyProperty.Register(nameof(ValidateRowDeletionCommand), typeof(ICommand<ValidateRowDeletionArgs>), typeof(UndoBehavior), new PropertyMetadata(null));

        public ICommand<ValidateRowDeletionArgs> ValidateRowDeletionCommand {
            get { return (ICommand<ValidateRowDeletionArgs>)GetValue(ValidateRowDeletionCommandProperty); }
            set { SetValue(ValidateRowDeletionCommandProperty, value); }
        }

        public ICommand UndoCommand { get; private set; }

        public UndoBehavior() {
            UndoCommand = new DelegateCommand(Undo, CanUndo);
        }

        IList source;

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.ValidateRow += OnRowAddedOrEdited;
            AssociatedObject.ValidateRowDeletion += OnRowDeleted;
            AssociatedObject.RowEditStarted += OnEditingStarted;
            AssociatedObject.DataSourceRefresh += OnRefresh;
            AssociatedObject.InitNewRow += OnNewRowStarted;
            source = (IList)AssociatedObject.DataControl.ItemsSource;
        }

        private void OnRefresh(object sender, DataSourceRefreshEventArgs e) {
            undoAction = null;
            editingCache = null;
        }

        protected override void OnDetaching() {
            AssociatedObject.ValidateRow -= OnRowAddedOrEdited;
            AssociatedObject.ValidateRowDeletion -= OnRowDeleted;
            AssociatedObject.RowEditStarted -= OnEditingStarted;
            AssociatedObject.DataSourceRefresh -= OnRefresh;
            AssociatedObject.InitNewRow -= OnNewRowStarted;
            source = null;
            base.OnDetaching();
        }

        private void OnNewRowStarted(object sender, InitNewRowEventArgs e) {
            isNewItemRowEditing = true;
        }

        bool isNewItemRowEditing;

        object editingCache;

        private void OnEditingStarted(object sender, RowEditStartedEventArgs e) {
            if(e.RowHandle != DataControlBase.NewItemRowHandle) {
                editingCache = CopyOperationsSupporter.Clone(e.Row);
            }
        }

        private void OnRowDeleted(object sender, GridValidateRowDeletionEventArgs e) {
            undoAction = new Action(() => {
                InsertItem(e.RowHandles.Single(), CopyOperationsSupporter.Clone(e.Rows.Single())); //Somehow index is changing after a new adding to source
            });
        }

        private void OnRowAddedOrEdited(object sender, GridRowValidationEventArgs e) {
            var item = e.Row;
            undoAction = e.IsNewItem ? new Action(() => RemoveItem(item)) : new Action(() => ApplyEditingCache(item));
            isNewItemRowEditing = false;
        }

        void ApplyEditingCache(object item) {
            CopyOperationsSupporter.CopyTo(editingCache, item);
            AssociatedObject.DataControl.RefreshRow(source.IndexOf(item)); //TODO: find a way how to get row handle by element or list index
            ValidateRowCommand.Execute(new RowValidationArgs(editingCache, source.IndexOf(item), false, new CancellationToken(), false));
        }

        Action undoAction;

        void Undo() {
            undoAction?.Invoke();
            undoAction = null;
        }

        bool CanUndo() {
            return undoAction != null && !AssociatedObject.IsEditing && !isNewItemRowEditing && !AssociatedObject.IsDataSourceRefreshing;
        }

        void RemoveItem(object item) {
            source.Remove(item);
            ValidateRowDeletionCommand.Execute(new ValidateRowDeletionArgs(new object[] { item }, new int[] { source.IndexOf(item) }));
        }

        void InsertItem(int position, object item) {
            source.Insert(position, item);
            ValidateRowCommand.Execute(new RowValidationArgs(item, source.IndexOf(item), true, new CancellationToken(), false));
        }

    }
}
