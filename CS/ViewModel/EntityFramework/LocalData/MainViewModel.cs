using DevExpress.Mvvm;
using EntityFrameworkIssues.Issues;
using System.Data.Entity;
using DevExpress.Mvvm.DataAnnotations;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Mvvm.Xpf;

namespace EntityFrameworkIssues {
    public class MainViewModel : ViewModelBase {
        IssuesContext _Context;
        IList<User> _ItemsSource;
        public IList<User> ItemsSource {
            get
            {
                if(_ItemsSource == null && !DevExpress.Mvvm.ViewModelBase.IsInDesignMode) {
                    _Context = new IssuesContext();
                    _ItemsSource = _Context.Users.ToList();
                }
                return _ItemsSource;
            }
        }
        [Command]
        public void ValidateRow(RowValidationArgs args) {
            var item = (User)args.Item;
            if(args.IsNewItem)
                _Context.Users.Add(item);
            _Context.SaveChanges();
        }
        [Command]
        public void ValidateRowDeletion(ValidateRowDeletionArgs args) {
            var item = (User)args.Items.Single();
            _Context.Users.Remove(item);
            _Context.SaveChanges();
        }
        [Command]
        public void DataSourceRefresh(DataSourceRefreshArgs args) {
            _ItemsSource = null;
            _Context = null;
            RaisePropertyChanged(nameof(ItemsSource));
        }
    }
}