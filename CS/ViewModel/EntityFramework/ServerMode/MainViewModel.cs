using DevExpress.Mvvm;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using EntityFrameworkIssues.Issues;
using DevExpress.Mvvm.Xpf;
using System;

namespace EntityFrameworkIssues {
    public class MainViewModel : ViewModelBase {
        DevExpress.Data.Linq.EntityServerModeSource _ServerModeSource;

        public DevExpress.Data.Linq.EntityServerModeSource ServerModeSource
        {
            get
            {
                if(_ServerModeSource == null) {
                    var context = new EntityFrameworkIssues.Issues.IssuesContext();
                    _ServerModeSource = new DevExpress.Data.Linq.EntityServerModeSource
                    {
                        KeyExpression = nameof(EntityFrameworkIssues.Issues.Issue.Id),
                        QueryableSource = context.Issues.AsNoTracking()
                    };
                }
                return _ServerModeSource;
            }
        }
        System.Collections.IList _Users;

        public System.Collections.IList Users
        {
            get
            {
                if(_Users == null && !IsInDesignMode) {
                    var context = new EntityFrameworkIssues.Issues.IssuesContext();
                    _Users = context.Users.Select(user => new { Id = user.Id, Name = user.FirstName + " " + user.LastName }).ToArray();
                }
                return _Users;
            }
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void RefreshDataSource(DevExpress.Mvvm.Xpf.RefreshDataSourceArgs args) {
            _Users = null;
            RaisePropertyChanged(nameof(Users));
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void CreateEditEntityViewModel(DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs args) {
            var context = new IssuesContext();
            Issue item;
            if(args.Key != null)
                item = context.Issues.Find(args.Key);
            else {
                item = new Issue() { Created = DateTime.Now };
                context.Entry(item).State = EntityState.Added;
            }
            args.ViewModel = new EditItemViewModel(item, new EditIssueInfo(context, Users));
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRow(DevExpress.Mvvm.Xpf.EditFormRowValidationArgs args) {
            var context = ((EditIssueInfo)args.Tag).Context;
            context.SaveChanges();
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateDeleteRows(DevExpress.Mvvm.Xpf.EditFormDeleteRowsValidationArgs args) {
            var key = (int)args.Keys.Single();
            var item = new Issue() { Id = key };
            var context = new IssuesContext();
            context.Entry(item).State = EntityState.Deleted;
            context.SaveChanges();
        }
    }
}