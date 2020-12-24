using System;
using System.Data.Entity;
using System.Linq;

namespace GridControlCRUDMVVMInfiniteAsyncSource {
    public class IssuesContextInitializer
        : DropCreateDatabaseIfModelChanges<IssuesContext> {
        //: DropCreateDatabaseAlways<IssuesContext> { 

        protected override void Seed(IssuesContext context) {
            base.Seed(context);
            var users = OutlookDataGenerator.Users
                .Select(x => {
                    var split = x.Split(' ');
                    return new User() {
                        FirstName = split[0],
                        LastName = split[1]
                    }; 
                })
                .ToArray();
            context.Users.AddRange(users);
            context.SaveChanges();

            var rnd = new Random();
            var issues = Enumerable.Range(0, 1000)
                .Select(i => new Issue() {
                    Subject = OutlookDataGenerator.GetSubject(),
                    UserId = users[rnd.Next(users.Length)].Id,
                    Created = DateTime.Today.AddDays(-rnd.Next(30) - 1),
                    Priority = OutlookDataGenerator.GetPriority(),
                    Votes = rnd.Next(100),
                })
                .ToArray();
            context.Issues.AddRange(issues);

            context.SaveChanges();
        }
    }
}
