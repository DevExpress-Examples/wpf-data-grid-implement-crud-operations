using DevExpress.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace EFCoreIssues {
    public class MainViewModel : ViewModelBase {
        EFCoreIssues.Issues.IssuesContext _Context;
        ObservableCollection<EFCoreIssues.Issues.User> _ItemsSource;

        UserCopyOperationsSupporter _CopyOperationsSupporter;
        public UserCopyOperationsSupporter CopyOperationsSupporter {
            get {
                if(_CopyOperationsSupporter == null) {
                    _CopyOperationsSupporter = new UserCopyOperationsSupporter();
                }
                return _CopyOperationsSupporter;
            }
        }

        public ObservableCollection<EFCoreIssues.Issues.User> ItemsSource
        {
            get
            {
                if(_ItemsSource == null && !IsInDesignMode) {
                    _Context = new EFCoreIssues.Issues.IssuesContext();
                    _ItemsSource = new ObservableCollection<Issues.User>(_Context.Users);
                }
                return _ItemsSource;
            }
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRow(DevExpress.Mvvm.Xpf.RowValidationArgs args) {
            var item = (EFCoreIssues.Issues.User)args.Item;
            if(args.IsNewItem)
                _Context.Users.Add(item);
            _Context.SaveChanges();
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRowDeletion(DevExpress.Mvvm.Xpf.ValidateRowDeletionArgs args) {
            var item = (EFCoreIssues.Issues.User)args.Items.Single();
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