using DevExpress.CRUD.DataModel;

namespace GridControlCRUDMVVMInfiniteAsyncSource {
    public class IssuesDataStorage {
        public IssuesDataStorage(ICRUDDataProvider<IssueData> issues, IDataProvider<User> users) {
            Users = users;
            Issues = issues;
        }
        public IDataProvider<User> Users { get; }
        public ICRUDDataProvider<IssueData> Issues { get; }
    }
}
