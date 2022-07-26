Imports System.Collections.Generic
Imports System.Data.Entity

Namespace DevExpress.CRUD.Northwind

    Public Class NorthwindContextInitializer
        Inherits DropCreateDatabaseIfModelChanges(Of NorthwindContext)

        Protected Overrides Sub Seed(ByVal context As NorthwindContext)
            MyBase.Seed(context)
            Dim categories = New List(Of Category) From {New Category With {.Name = "Beverages", .Description = "Soft drinks, coffees, teas, beers, and ales", .Products = New List(Of Product) From {New Product With {.Name = "Chai", .QuantityPerUnit = "10 boxes x 20 bags", .UnitPrice = CDec(18), .UnitsInStock = 39, .UnitsOnOrder = 0, .ReorderLevel = 10, .Discontinued = False, .EAN13 = "070684900001"}, New Product With {.Name = "Ipoh Coffee", .QuantityPerUnit = "16 - 500 g tins", .UnitPrice = CDec(46), .UnitsInStock = 17, .UnitsOnOrder = 10, .ReorderLevel = 25, .Discontinued = False, .EAN13 = "070684900043"}}}, New Category With {.Name = "Condiments", .Description = "Sweet and savory sauces, relishes, spreads, and seasonings", .Products = New List(Of Product) From {New Product With {.Name = "Aniseed Syrup", .QuantityPerUnit = "12 - 550 ml bottles", .UnitPrice = CDec(10), .UnitsInStock = 13, .UnitsOnOrder = 70, .ReorderLevel = 25, .Discontinued = False, .EAN13 = "070684900003"}, New Product With {.Name = "Louisiana Fiery Hot Pepper Sauce", .QuantityPerUnit = "32 - 8 oz bottles", .UnitPrice = CDec(21.05), .UnitsInStock = 76, .UnitsOnOrder = 0, .ReorderLevel = 0, .Discontinued = False, .EAN13 = "070684900065"}}}, New Category With {.Name = "Grains/Cereals", .Description = "Breads, crackers, pasta, and cereal", .Products = New List(Of Product) From {New Product With {.Name = "Singaporean Hokkien Fried Mee", .QuantityPerUnit = "32 - 1 kg pkgs.", .UnitPrice = CDec(14), .UnitsInStock = 26, .UnitsOnOrder = 0, .ReorderLevel = 0, .Discontinued = True, .EAN13 = "070684900042"}, New Product With {.Name = "Ravioli Angelo", .QuantityPerUnit = "24 - 250 g pkgs.", .UnitPrice = CDec(19.5), .UnitsInStock = 36, .UnitsOnOrder = 0, .ReorderLevel = 20, .Discontinued = False, .EAN13 = "070684900057"}}}}
            context.Categories.AddRange(categories)
            context.SaveChanges()
        End Sub
    End Class
End Namespace
