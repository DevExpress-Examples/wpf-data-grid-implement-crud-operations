using System;
using System.Linq;

namespace GridControlCRUDMVVMInfiniteAsyncSource {
    public class IssueData {
        public IssueData() {
            Created = DateTime.Today;
        }
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime Created { get; set; }
        public int Votes { get; set; }
        public Priority Priority { get; set; }
        public int UserId { get; set; }
    }
}
