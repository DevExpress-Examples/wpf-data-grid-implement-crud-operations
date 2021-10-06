using DevExpress.Mvvm;
using System.Linq;

namespace EntityFrameworkIssues {
    public class MainViewModel : ViewModelBase {
        EntityFrameworkIssues.Issues.IssuesContext _Context;
        System.Collections.Generic.IList<EntityFrameworkIssues.Issues.User> _ItemsSource;

        public System.Collections.Generic.IList<EntityFrameworkIssues.Issues.User> ItemsSource
        {
            get
            {
                if(_ItemsSource == null && !IsInDesignMode) {
                    _Context = new EntityFrameworkIssues.Issues.IssuesContext();
                    _ItemsSource = _Context.Users.ToList();
                }
                return _ItemsSource;
            }
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRow(DevExpress.Mvvm.Xpf.RowValidationArgs args) {
            var item = (EntityFrameworkIssues.Issues.User)args.Item;
            if(args.IsNewItem)
                _Context.Users.Add(item);
            _Context.SaveChanges();
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateDeleteRows(DevExpress.Mvvm.Xpf.DeleteRowsValidationArgs args) {
            var item = (EntityFrameworkIssues.Issues.User)args.Items.Single();
            _Context.Users.Remove(item);
            _Context.SaveChanges();
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void RefreshDataSource(DevExpress.Mvvm.Xpf.RefreshDataSourceArgs args) {
            _ItemsSource = null;
            _Context = null;
            RaisePropertyChanged(nameof(ItemsSource));
        }
    }
}