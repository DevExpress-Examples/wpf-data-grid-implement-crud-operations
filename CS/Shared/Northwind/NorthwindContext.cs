using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.CRUD.Northwind {
    public class NorthwindContext : DbContext {
        static NorthwindContext() {
            Database.SetInitializer(new NorthwindContextInitializer());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<Product>().Property(x => x.Name).IsRequired();
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
