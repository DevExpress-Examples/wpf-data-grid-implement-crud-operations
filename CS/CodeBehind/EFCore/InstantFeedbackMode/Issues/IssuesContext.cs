using Microsoft.EntityFrameworkCore;

namespace EFCoreIssues.Issues {
    public class IssuesContext : DbContext {
        static readonly DbContextOptions<IssuesContext> options = new DbContextOptionsBuilder<IssuesContext>()
           .UseInMemoryDatabase(databaseName: "Test")
           .Options;
        public IssuesContext()
            : base(options) {
        }

        public DbSet<Issue> Issues { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
