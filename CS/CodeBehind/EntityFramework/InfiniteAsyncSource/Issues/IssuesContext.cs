using System.Data.Entity;

namespace EntityFrameworkIssues.Issues {
    public class IssuesContext : DbContext {
        static IssuesContext() {
            Database.SetInitializer(new IssuesContextInitializer());
        }
        public IssuesContext() { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Issue>()
                .HasIndex(x => x.Created);

            modelBuilder.Entity<Issue>()
                .HasIndex(x => x.Votes);
        }

        public DbSet<Issue> Issues { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
