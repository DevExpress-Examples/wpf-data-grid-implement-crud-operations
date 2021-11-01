using DevExpress.Mvvm;
using System.Collections.Generic;

namespace EFCoreIssues.Issues {
    public class User : BindableBase {
        public int Id { get => GetValue<int>(); set => SetValue(value); }
        public string FirstName { get => GetValue<string>(); set => SetValue(value); }
        public string LastName { get => GetValue<string>(); set => SetValue(value); }
        public virtual ICollection<Issue> Issues { get => GetValue<ICollection<Issue>>(); set => SetValue(value); }

        public User Clone() {
            return new User() { FirstName = FirstName, Id = Id, Issues = Issues, LastName = LastName };
        }

        public void CopyTo(User anotherUser) {
            anotherUser.FirstName = FirstName;
            anotherUser.Id = Id;
            anotherUser.Issues = Issues;
            anotherUser.LastName = LastName;
        }
    }
}
