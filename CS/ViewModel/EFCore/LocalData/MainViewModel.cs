using DevExpress.Mvvm;
using System.Linq;

namespace EFCoreIssues {
    public class MainViewModel : ViewModelBase {
        EFCoreIssues.Issues.IssuesContext _Context;
        System.Collections.Generic.IList<EFCoreIssues.Issues.User> _ItemsSource;

        public System.Collections.Generic.IList<EFCoreIssues.Issues.User> ItemsSource
        {
            get
            {
                if(_ItemsSource == null && !IsInDesignMode) {
                    _Context = new EFCoreIssues.Issues.IssuesContext();
                    _ItemsSource = _Context.Users.ToList();
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
        public void ValidateRowDeletion(DevExpress.Mvvm.Xpf.DeleteRowsValidationArgs args) {
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