using EFCoreIssues.Issues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreIssues {
    public class UserCopyOperationsSupporter : IDataItemCopyOperationsSupporter {
        public object Clone(object item) {
            var userItem = GetUser(item);
            return new User() { FirstName = userItem.FirstName, Id = userItem.Id, Issues = userItem.Issues, LastName = userItem.LastName };
        }

        public void CopyTo(object source, object target) {
            var userSource = GetUser(source);
            var userTarget = GetUser(target);
            userTarget.FirstName = userSource.FirstName;
            userTarget.Id = userSource.Id;
            userTarget.Issues = userSource.Issues;
            userTarget.LastName = userSource.LastName;
        }

        User GetUser(object item) {
            var result = item as User;
            if(result == null) {
                throw new InvalidOperationException();
            }
            return result;
        }
    }
}
