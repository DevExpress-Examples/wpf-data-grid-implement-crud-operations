using System.Collections.Generic;
using System.Data.Entity;

namespace DevExpress.CRUD.Northwind {
    public class NorthwindContextInitializer : DropCreateDatabaseIfModelChanges<NorthwindContext> {
        protected override void Seed(NorthwindContext context) {
            base.Seed(context);

            var categories = new List<Category> {
                new Category {
                    Name = "Beverages",
                    Description = "Soft drinks, coffees, teas, beers, and ales",
                    Products = new List<Product> {
                        new Product { Name = "Chai", QuantityPerUnit = "10 boxes x 20 bags", UnitPrice = (decimal)18, UnitsInStock = 39, UnitsOnOrder = 0, ReorderLevel = 10, Discontinued = false, EAN13 = "070684900001"  },
                        new Product { Name = "Ipoh Coffee", QuantityPerUnit = "16 - 500 g tins", UnitPrice = (decimal)46, UnitsInStock = 17, UnitsOnOrder = 10, ReorderLevel = 25, Discontinued = false, EAN13 = "070684900043"  },
                    }
                },
                new Category {
                    Name = "Condiments",
                    Description = "Sweet and savory sauces, relishes, spreads, and seasonings",
                    Products = new List<Product> {
                        new Product { Name = "Aniseed Syrup", QuantityPerUnit = "12 - 550 ml bottles", UnitPrice = (decimal)10, UnitsInStock = 13, UnitsOnOrder = 70, ReorderLevel = 25, Discontinued = false, EAN13 = "070684900003"  },
                        new Product { Name = "Louisiana Fiery Hot Pepper Sauce", QuantityPerUnit = "32 - 8 oz bottles", UnitPrice = (decimal)21.05, UnitsInStock = 76, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false, EAN13 = "070684900065"  },
                    }
                },
                new Category {
                    Name = "Grains/Cereals",
                    Description = "Breads, crackers, pasta, and cereal",
                    Products = new List<Product> {
                        new Product { Name = "Singaporean Hokkien Fried Mee", QuantityPerUnit = "32 - 1 kg pkgs.", UnitPrice = (decimal)14, UnitsInStock = 26, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = true, EAN13 = "070684900042"  },
                        new Product { Name = "Ravioli Angelo", QuantityPerUnit = "24 - 250 g pkgs.", UnitPrice = (decimal)19.5, UnitsInStock = 36, UnitsOnOrder = 0, ReorderLevel = 20, Discontinued = false, EAN13 = "070684900057"  },
                    }
                },
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}
