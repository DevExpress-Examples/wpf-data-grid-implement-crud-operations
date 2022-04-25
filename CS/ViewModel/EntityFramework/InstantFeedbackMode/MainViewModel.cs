using DevExpress.Mvvm;
using EntityFrameworkIssues.Issues;
using System.Data.Entity;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm.Xpf;
using System;

namespace EntityFrameworkIssues {
    public class MainViewModel : ViewModelBase {
        DevExpress.Data.Linq.EntityInstantFeedbackSource _ItemsSource;
        public DevExpress.Data.Linq.EntityInstantFeedbackSource ItemsSource {
            get
            {
                if(_ItemsSource == null) {
                    _ItemsSource = new DevExpress.Data.Linq.EntityInstantFeedbackSource {
                        KeyExpression = nameof(Issue.Id)
                    };
                    _ItemsSource.GetQueryable += (sender, e) => {
                        var context = new IssuesContext();
                        e.QueryableSource = context.Issues.AsNoTracking();
                    };
                }
                return _ItemsSource;
            }
        }
        System.Collections.IList _Users;
        public System.Collections.IList Users {
            get
            {
                if(_Users == null && !DevExpress.Mvvm.ViewModelBase.IsInDesignMode) {
                    var context = new EntityFrameworkIssues.Issues.IssuesContext();
                    _Users = context.Users.Select(user => new { Id = user.Id, Name = user.FirstName + " " + user.LastName }).ToArray();
                }
                return _Users;
            }
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void DataSourceRefresh(DevExpress.Mvvm.Xpf.DataSourceRefreshArgs args) {
            _Users = null;
            RaisePropertyChanged(nameof(Users));
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void CreateEditEntityViewModel(DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs args) {
            var context = new IssuesContext();
            Issue item;
            if(args.IsNewItem) {
                item = new Issue() { Created = DateTime.Now };
                context.Entry(item).State = EntityState.Added;
            } else {
                item = context.Issues.Find(args.Key);
            }
            args.ViewModel = new EditItemViewModel(
                item,
                new EditIssueInfo(context, Users),
                title: (args.IsNewItem ? "New " : "Edit ") + nameof(Issue)
            );
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRow(DevExpress.Mvvm.Xpf.EditFormRowValidationArgs args) {
            var context = ((EditIssueInfo)args.EditOperationContext).DbContext;
            context.SaveChanges();
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRowDeletion(DevExpress.Mvvm.Xpf.EditFormValidateRowDeletionArgs args) {
            var key = (int)args.Keys.Single();
            var item = new Issue() { Id = key };
            var context = new IssuesContext();
            context.Entry(item).State = EntityState.Deleted;
            context.SaveChanges();
        }
    }
}