using DevExpress.CRUD.DataModel;

namespace GridControlCRUDMVVMInfiniteAsyncSource {
    public class IssuesDataStorage {
        public IssuesDataStorage(IDataProvider<IssueData> issues, IDataProvider<User> users) {
            Users = users;
            Issues = issues;
        }
        public IDataProvider<User> Users { get; }
        public IDataProvider<IssueData> Issues { get; }
    }
}
