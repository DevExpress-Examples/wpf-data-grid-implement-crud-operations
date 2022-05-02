using DevExpress.Mvvm;
using XPOIssues.Issues;
using DevExpress.Mvvm.DataAnnotations;
using System;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo;
using System.ComponentModel;
using DevExpress.Mvvm.Xpf;
using System.Collections;

namespace XPOIssues {
    public class MainViewModel : ViewModelBase {
        DetachedObjectsHelper<Issue> _DetachedObjectsHelper;
        public DetachedObjectsHelper<Issue> DetachedObjectsHelper {
            get
            {
                if(_DetachedObjectsHelper == null) {
                    using(var session = new Session()) {
                        var classInfo = session.GetClassInfo<Issue>();
                        var properties = classInfo.Members
                            .Where(member => member.IsPublic && member.IsPersistent)
                            .Select(member => member.Name)
                            .ToArray();
                        _DetachedObjectsHelper = DetachedObjectsHelper<Issue>.Create(classInfo.KeyProperty.Name, properties);
                    }
                }
                return _DetachedObjectsHelper;
            }
        }
        public PropertyDescriptorCollection Properties {
            get
            {
                return DetachedObjectsHelper.Properties;
            }
        }

        Expression<Func<Issue, bool>> MakeFilterExpression(CriteriaOperator filter) {
            var converter = new GridFilterCriteriaToExpressionConverter<Issue>();
            return converter.Convert(filter);
        }
        [Command]
        public void FetchRows(FetchRowsAsyncArgs args) {
            args.Result = Task.Run<FetchRowsResult>(() => {
                using(var session = new Session()) {
                    var queryable = session.Query<Issue>().SortBy(args.SortOrder, defaultUniqueSortPropertyName: nameof(Issue.Oid)).Where(MakeFilterExpression((CriteriaOperator)args.Filter));
                    var items = queryable.Skip(args.Skip).Take(args.Take ?? 100).ToArray();
                    return DetachedObjectsHelper.ConvertToDetachedObjects(items);
                }
            });
        }
        [Command]
        public void GetTotalSummaries(GetSummariesAsyncArgs args) {
            args.Result = Task.Run(() => {
                using(var session = new Session()) {
                    return session.Query<Issue>().Where(MakeFilterExpression((CriteriaOperator)args.Filter)).GetSummaries(args.Summaries);
                }
            });
        }
        [Command]
        public void ValidateRow(RowValidationArgs args) {
            using(var unitOfWork = new UnitOfWork()) {
                var item = args.IsNewItem
                    ? new Issue(unitOfWork)
                    : unitOfWork.GetObjectByKey<Issue>(DetachedObjectsHelper.GetKey(args.Item));
                DetachedObjectsHelper.ApplyProperties(item, args.Item);
                unitOfWork.CommitChanges();
                if(args.IsNewItem) {
                    DetachedObjectsHelper.SetKey(args.Item, item.Oid);
                }
            }
        }
        [Command]
        public void ValidateRowDeletion(ValidateRowDeletionArgs args) {
            using(var unitOfWork = new UnitOfWork()) {
                var key = DetachedObjectsHelper.GetKey(args.Items.Single());
                var item = unitOfWork.GetObjectByKey<Issue>(key);
                unitOfWork.Delete(item);
                unitOfWork.CommitChanges();
            }
        }
        IList _Users;
        public IList Users {
            get
            {
                if(_Users == null && !DevExpress.Mvvm.ViewModelBase.IsInDesignMode) {
                    {
                        var session = new Session();
                        _Users = session.Query<User>().OrderBy(user => user.Oid).Select(user => new { Id = user.Oid, Name = user.FirstName + " " + user.LastName }).ToArray();
                    }
                }
                return _Users;
            }
        }
        [Command]
        public void DataSourceRefresh(DataSourceRefreshArgs args) {
            _Users = null;
            RaisePropertyChanged(nameof(Users));
        }
    }
}