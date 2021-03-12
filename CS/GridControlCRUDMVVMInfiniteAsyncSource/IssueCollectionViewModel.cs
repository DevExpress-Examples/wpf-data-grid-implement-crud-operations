using DevExpress.CRUD.DataModel;
using DevExpress.CRUD.ViewModel;
using GridControlCRUDMVVMInfiniteAsyncSource;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridControlCRUDMVVMInfiniteAsyncSource {
    public class IssueCollectionViewModel : VirtualCollectionViewModel<IssueData> {
        public IList<User> Users {
            get => GetValue<IList<User>>();
            private set => SetValue(value);
        }

        readonly IDataProvider<User> usersDataProvider;

        public IssueCollectionViewModel() 
            : this(IssuesDataStorageFactory.Create(IsInDesignMode)) {
        }

        public IssueCollectionViewModel(IssuesDataStorage dataStorage) 
            : base(dataStorage.Issues) {
            usersDataProvider = dataStorage.Users;
            RefreshUsers();
        }

        async void RefreshUsers() {
            await OnRefreshCoreAsync();
        }
        protected override async Task OnRefreshCoreAsync() {
            if(usersDataProvider != null) {
                try {
                    Users = await usersDataProvider.ReadAsync();
                } catch {
                    Users = null;
                }
            }
        }
        protected override EntityViewModel<IssueData> CreateEntityViewModel(IssueData entity) {
            return new IssueDataViewModel(entity, usersDataProvider.ReadAsync());
        }
    }
    public class IssueDataViewModel : EntityViewModel<IssueData> {
        public IssueDataViewModel(IssueData entity, Task<IList<User>> usersTask) : base(entity) {
            AssignUsers(usersTask);
        }
        async void AssignUsers(Task<IList<User>> usersTask) {
            Users = await usersTask;
        }
        public IList<User> Users {
            get => GetValue<IList<User>>();
            private set => SetValue(value);
        }
    }
}
