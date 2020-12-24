using System.Collections.Generic;

namespace GridControlCRUDMVVMInfiniteAsyncSource {
    public class User {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName + " " + LastName;
        public virtual ICollection<Issue> Issues { get; set; }
    }
}
