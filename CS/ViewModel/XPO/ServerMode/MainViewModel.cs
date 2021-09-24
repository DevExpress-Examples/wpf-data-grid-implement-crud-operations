using DevExpress.Mvvm;
using System.Linq;
using DevExpress.Xpo;
using XPOIssues.Issues;
using DevExpress.Mvvm.Xpf;
using System;

namespace XPOIssues {
    public class MainViewModel : ViewModelBase {
        DevExpress.Xpo.XPServerModeView _ServerModeSource;

        public DevExpress.Xpo.XPServerModeView ServerModeSource
        {
            get
            {
                if(_ServerModeSource == null) {
                    var properties = new DevExpress.Xpo.ServerViewProperty[] {
new DevExpress.Xpo.ServerViewProperty("Oid", DevExpress.Xpo.SortDirection.Ascending, new DevExpress.Data.Filtering.OperandProperty("Oid")),
new DevExpress.Xpo.ServerViewProperty("Subject", DevExpress.Xpo.SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("Subject")),
new DevExpress.Xpo.ServerViewProperty("UserId", DevExpress.Xpo.SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("UserId")),
new DevExpress.Xpo.ServerViewProperty("Created", DevExpress.Xpo.SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("Created")),
new DevExpress.Xpo.ServerViewProperty("Votes", DevExpress.Xpo.SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("Votes")),
new DevExpress.Xpo.ServerViewProperty("Priority", DevExpress.Xpo.SortDirection.None, new DevExpress.Data.Filtering.OperandProperty("Priority"))
    };
                    var session = new DevExpress.Xpo.Session();
                    _ServerModeSource = new DevExpress.Xpo.XPServerModeView(session, typeof(XPOIssues.Issues.Issue), null);
                    _ServerModeSource.Properties.AddRange(properties);
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
                    {
                        var session = new DevExpress.Xpo.Session();
                        _Users = session.Query<XPOIssues.Issues.User>().OrderBy(user => user.Oid).Select(user => new { Id = user.Oid, Name = user.FirstName + " " + user.LastName }).ToArray();
                    }
                }
                return _Users;
            }
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void Refresh(DevExpress.Mvvm.Xpf.RefreshArgs args) {
            _Users = null;
            RaisePropertyChanged(nameof(Users));
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void CreateEditEntityViewModel(DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs args) {
            var unitOfWork = new UnitOfWork();
            var item = args.Key != null
                ? unitOfWork.GetObjectByKey<Issue>(args.Key)
                : new Issue(unitOfWork);
            args.ViewModel = new EditItemViewModel(
                item,
                new EditIssueInfo(unitOfWork, Users),
                dispose: () => unitOfWork.Dispose()
            );
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRow(DevExpress.Mvvm.Xpf.EditFormRowValidationArgs args) {
            var unitOfWork = ((EditIssueInfo)args.Tag).UnitOfWork;
            unitOfWork.CommitChanges();
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateDeleteRows(DevExpress.Mvvm.Xpf.EditFormDeleteRowsValidationArgs args) {
            using(var unitOfWork = new UnitOfWork()) {
                var key = (int)args.Keys.Single();
                var item = unitOfWork.GetObjectByKey<Issue>(key);
                unitOfWork.Delete(item);
                unitOfWork.CommitChanges();
            }
        }
    }
}