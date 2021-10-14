using System;

namespace EntityFrameworkIssues.Issues {
    public class Issue {
        public int Id { get; set; }
        public string Subject { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public DateTime Created { get; set; }
        public int Votes { get; set; }
        public Priority Priority { get; set; }
        public Issue() {
            Created = DateTime.Now;
        }
    }
    public enum Priority { Low, BelowNormal, Normal, AboveNormal, High }
}
