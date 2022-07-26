Imports DevExpress.CRUD.DataModel.EntityFramework
Imports DevExpress.CRUD.DataModel

Namespace DevExpress.CRUD.Northwind.DataModel

    Public Module NorthwindDataStorageFactory

        Public Function Create(ByVal isInDesignMode As Boolean) As NorthwindDataStorage
            If isInDesignMode Then
                Return New DevExpress.CRUD.Northwind.DataModel.NorthwindDataStorage(New DevExpress.CRUD.DataModel.DesignTimeDataProvider(Of DevExpress.CRUD.Northwind.DataModel.CategoryInfo)(Function(id) New DevExpress.CRUD.Northwind.DataModel.CategoryInfo With {.Id = id, .Name = "Category " & id}), New DevExpress.CRUD.DataModel.DesignTimeDataProvider(Of DevExpress.CRUD.Northwind.DataModel.ProductInfo)(Function(id) New DevExpress.CRUD.Northwind.DataModel.ProductInfo With {.Id = id, .Name = "Product " & id, .CategoryId = id}))
            End If

            Return New DevExpress.CRUD.Northwind.DataModel.NorthwindDataStorage(New DevExpress.CRUD.DataModel.EntityFramework.EntityFrameworkDataProvider(Of DevExpress.CRUD.Northwind.NorthwindContext, DevExpress.CRUD.Northwind.Category, DevExpress.CRUD.Northwind.DataModel.CategoryInfo)(createContext:=Function() New DevExpress.CRUD.Northwind.NorthwindContext(), getDbSet:=Function(context) context.Categories, getEnityExpression:=Function(category) New DevExpress.CRUD.Northwind.DataModel.CategoryInfo With {.Id = category.Id, .Name = category.Name}), New DevExpress.CRUD.DataModel.EntityFramework.EntityFrameworkCRUDDataProvider(Of DevExpress.CRUD.Northwind.NorthwindContext, DevExpress.CRUD.Northwind.Product, DevExpress.CRUD.Northwind.DataModel.ProductInfo, Long)(createContext:=Function() New DevExpress.CRUD.Northwind.NorthwindContext(), getDbSet:=Function(context) context.Products, getEnityExpression:=Function(product) New DevExpress.CRUD.Northwind.DataModel.ProductInfo With {.Id = product.Id, .Name = product.Name, .CategoryId = product.CategoryId}, getKey:=Function(productInfo) productInfo.Id, getEntityKey:=Function(product) product.Id, setKey:=Sub(productInfo, id) productInfo.Id = id, applyProperties:=Sub(productInfo, product)
                product.Name = productInfo.Name
                product.CategoryId = productInfo.CategoryId
            End Sub))
        End Function
    End Module
End Namespace
