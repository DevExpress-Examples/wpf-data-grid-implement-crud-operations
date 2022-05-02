using DevExpress.Mvvm;
using EFCoreIssues.Issues;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Data.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections;
using DevExpress.Mvvm.Xpf;
using System;

namespace EFCoreIssues {
    public class MainViewModel : ViewModelBase {
        EntityServerModeSource _ItemsSource;
        public EntityServerModeSource ItemsSource {
            get
            {
                if(_ItemsSource == null) {
                    var context = new IssuesContext();
                    _ItemsSource = new EntityServerModeSource {
                        KeyExpression = nameof(Issue.Id),
                        QueryableSource = context.Issues.AsNoTracking()
                    };
                }
                return _ItemsSource;
            }
        }
        IList _Users;
        public IList Users {
            get
            {
                if(_Users == null && !DevExpress.Mvvm.ViewModelBase.IsInDesignMode) {
                    var context = new IssuesContext();
                    _Users = context.Users.Select(user => new { Id = user.Id, Name = user.FirstName + " " + user.LastName }).ToArray();
                }
                return _Users;
            }
        }
        [Command]
        public void DataSourceRefresh(DataSourceRefreshArgs args) {
            _Users = null;
            RaisePropertyChanged(nameof(Users));
        }
        [Command]
        public void CreateEditEntityViewModel(CreateEditItemViewModelArgs args) {
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
        [Command]
        public void ValidateRow(EditFormRowValidationArgs args) {
            var context = ((EditIssueInfo)args.EditOperationContext).DbContext;
            context.SaveChanges();
        }
        [Command]
        public void ValidateRowDeletion(EditFormValidateRowDeletionArgs args) {
            var key = (int)args.Keys.Single();
            var item = new Issue() { Id = key };
            var context = new IssuesContext();
            context.Entry(item).State = EntityState.Deleted;
            context.SaveChanges();
        }
    }
}