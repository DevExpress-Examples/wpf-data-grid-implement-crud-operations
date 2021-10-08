using DevExpress.Mvvm;
using System.Linq;

namespace XPOIssues {
    public class MainViewModel : ViewModelBase {
        DevExpress.Xpo.UnitOfWork _UnitOfWork;
        System.Collections.Generic.IList<XPOIssues.Issues.User> _ItemsSource;

        public System.Collections.Generic.IList<XPOIssues.Issues.User> ItemsSource
        {
            get
            {
                if(_ItemsSource == null && !IsInDesignMode) {
                    _UnitOfWork = new DevExpress.Xpo.UnitOfWork();
                    var xpCollection = new DevExpress.Xpo.XPCollection<XPOIssues.Issues.User>(_UnitOfWork);
                    xpCollection.Sorting.Add(new DevExpress.Xpo.SortProperty(nameof(XPOIssues.Issues.User.Oid), DevExpress.Xpo.DB.SortingDirection.Ascending));
                    _ItemsSource = xpCollection;
                }
                return _ItemsSource;
            }
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRow(DevExpress.Mvvm.Xpf.RowValidationArgs args) {
            _UnitOfWork.CommitChanges();
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRowDeletion(DevExpress.Mvvm.Xpf.DeleteRowsValidationArgs args) {
            var item = (XPOIssues.Issues.User)args.Items.Single();
            _UnitOfWork.Delete(item);
            _UnitOfWork.CommitChanges();
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void DataSourceRefresh(DevExpress.Mvvm.Xpf.DataSourceRefreshArgs args) {
            _ItemsSource = null;
            _UnitOfWork = null;
            RaisePropertyChanged(nameof(ItemsSource));
        }
    }
}