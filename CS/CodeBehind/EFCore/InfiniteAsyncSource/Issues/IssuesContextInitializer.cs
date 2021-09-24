using System;
using System.Linq;

namespace EFCoreIssues.Issues {
    public static class IssuesContextInitializer {
        public static void Seed() {
            var context = new IssuesContext();
            var users = OutlookDataGenerator.Users
                .Select(x =>
                {
                    var split = x.Split(' ');
                    return new User()
                    {
                        FirstName = split[0],
                        LastName = split[1]
                    };
                })
                .ToArray();
            context.Users.AddRange(users);
            context.SaveChanges();

            var rnd = new Random(0);
            var issues = Enumerable.Range(0, 1000)
                .Select(i => new Issue()
                {
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
