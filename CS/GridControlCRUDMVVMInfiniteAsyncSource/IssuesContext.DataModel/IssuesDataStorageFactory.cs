using DevExpress.CRUD.DataModel.EntityFramework;
using DevExpress.CRUD.DataModel;

namespace GridControlCRUDMVVMInfiniteAsyncSource {
    public static class IssuesDataStorageFactory {
        public static IssuesDataStorage Create(bool isInDesignMode) {
            if(isInDesignMode) { 
                return new IssuesDataStorage(
                    new DesignTimeDataProvider<IssueData>(
                        id => new IssueData {
                            Id = id,
                            Subject = "Subject " + id,
                        }
                    ),
                    new DesignTimeDataProvider<User>(
                        id => new User {
                            Id = id,
                            FirstName = "FirstName " + id,
                        }
                    )
                );
            }
            return new IssuesDataStorage(
                new EntityFrameworkCRUDDataProvider<IssuesContext, Issue, IssueData, int>(
                    createContext: () => new IssuesContext(),
                    getDbSet: context => context.Issues,
                    getEnityExpression: x => new IssueData() {
                        Id = x.Id,
                        Subject = x.Subject,
                        UserId = x.UserId,
                        Created = x.Created,
                        Votes = x.Votes,
                        Priority = x.Priority,
                    },
                    getKey: ussueData => ussueData.Id,
                    getEntityKey: ussue => ussue.Id,
                    setKey: (ussueData, id) => ussueData.Id = id,
                    applyProperties: (ussueData, issue) => {
                        issue.Subject = ussueData.Subject;
                        issue.UserId = ussueData.UserId;
                        issue.Created = ussueData.Created;
                        issue.Votes = ussueData.Votes;
                        issue.Priority = ussueData.Priority;
                    },
                    keyProperty: nameof(IssueData.Id)
                ),
                new EntityFrameworkDataProvider<IssuesContext, User, User>(
                    createContext: () => new IssuesContext(),
                    getDbSet: context => context.Users,
                    getEnityExpression: user => user,
                    keyProperty: nameof(User.Id)
                )
            );
        }
    }
}
