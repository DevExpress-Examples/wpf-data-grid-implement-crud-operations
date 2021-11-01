using DevExpress.Mvvm;
using EFCoreIssues.Issues;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace EFCoreIssues {
    public class MainViewModel : ViewModelBase {
        EFCoreIssues.Issues.IssuesContext _Context;
        ObservableCollection<EFCoreIssues.Issues.User> _ItemsSource;

        public ObservableCollection<EFCoreIssues.Issues.User> ItemsSource
        {
            get
            {
                if(_ItemsSource == null && !IsInDesignMode) {
                    _Context = new EFCoreIssues.Issues.IssuesContext();
                    _ItemsSource = new ObservableCollection<User>(_Context.Users);
                }
                return _ItemsSource;
            }
        }

        void RemoveItem(User item) {
            ItemsSource.Remove(item);
            _Context.Users.Remove(item);
            _Context.SaveChanges();
        }

        void InsertItem(int position, User item) {
            ItemsSource.Insert(position, item);
            _Context.Users.Add(item);
            _Context.SaveChanges();
        }

        void ApplyEditingCache(User item) {
            editingCache.CopyTo(item);
            _Context.SaveChanges();
        }

        Action undoAction;
        User editingCache;


        [DevExpress.Mvvm.DataAnnotations.Command]
        public void Undo() {
            undoAction?.Invoke();
            undoAction = null;
        }

        public bool CanUndo() {
            return undoAction != null;
        }

        [DevExpress.Mvvm.DataAnnotations.Command]
        public void StartRowEdit(DevExpress.Mvvm.Xpf.RowEditStartedArgs args) {
            if(args.Item != null) {
                editingCache = ((User)args.Item).Clone();
            }
        }

        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRow(DevExpress.Mvvm.Xpf.RowValidationArgs args) {
            var item = (EFCoreIssues.Issues.User)args.Item;
            
            undoAction = args.IsNewItem ? new Action(() => RemoveItem(item)) : new Action(() => ApplyEditingCache(item));

            if(args.IsNewItem) {
                _Context.Users.Add(item);
            }
            _Context.SaveChanges();
        }

        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRowDeletion(DevExpress.Mvvm.Xpf.ValidateRowDeletionArgs args) {
            var item = (EFCoreIssues.Issues.User)args.Items.Single();

            undoAction = new Action(() => InsertItem(args.SourceIndexes.Single(), item.Clone()));

            _Context.Users.Remove(item);
            _Context.SaveChanges();
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void DataSourceRefresh(DevExpress.Mvvm.Xpf.DataSourceRefreshArgs args) {
            _ItemsSource = null;
            _Context = null;
            RaisePropertyChanged(nameof(ItemsSource));
        }
    }
}