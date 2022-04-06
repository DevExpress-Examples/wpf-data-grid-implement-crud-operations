using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XPOIssues.Issues {
    public static class DemoDataHelper {
        public static void Seed() {
            using(var uow = new DevExpress.Xpo.UnitOfWork()) {
                var users = OutlookDataGenerator.Users
                    .Select(x => {
                        var split = x.Split(' ');
                        return new User(uow) {
                            FirstName = split[0],
                            LastName = split[1]
                        };
                    })
                    .ToArray();
                uow.CommitChanges();
                var rnd = new Random(0);
                var issues = Enumerable.Range(0, 1000)
                    .Select(i => new Issue(uow) {
                        Subject = OutlookDataGenerator.GetSubject(),
                        UserId = users[rnd.Next(users.Length)].Oid,
                        Created = DateTime.Today.AddDays(-rnd.Next(30)),
                        Priority = OutlookDataGenerator.GetPriority(),
                        Votes = rnd.Next(100),
                    })
                    .ToArray();
                uow.CommitChanges();
            }
        }
    }
}
