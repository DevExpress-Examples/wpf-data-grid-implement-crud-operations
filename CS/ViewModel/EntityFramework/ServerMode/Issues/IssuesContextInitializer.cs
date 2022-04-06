using System;
using System.Data.Entity;
using System.Linq;

namespace EntityFrameworkIssues.Issues {
    public class IssuesContextInitializer
        : DropCreateDatabaseIfModelChanges<IssuesContext> {
        //: DropCreateDatabaseAlways<IssuesContext> { 

        public static void ResetData() {
            using(var context = new IssuesContext()) {
                context.Users.Load();
                context.Users.RemoveRange(context.Users);
                context.SaveChanges();
                CreateData(context);
            }
        }

        protected override void Seed(IssuesContext context) {
            base.Seed(context);
            CreateData(context);
        }

        static void CreateData(IssuesContext context) {
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

            var rnd = new Random(0);
            var issues = Enumerable.Range(0, 1000)
                .Select(i => new Issue() {
                    Subject = OutlookDataGenerator.GetSubject(),
                    UserId = users[rnd.Next(users.Length)].Id,
                    Created = DateTime.Today.AddDays(-rnd.Next(30)),
                    Priority = OutlookDataGenerator.GetPriority(),
                    Votes = rnd.Next(100),
                })
                .ToArray();
            context.Issues.AddRange(issues);

            context.SaveChanges();
        }
    }
}
