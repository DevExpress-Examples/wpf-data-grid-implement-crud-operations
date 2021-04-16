Imports DevExpress.CRUD.DataModel.EntityFramework
Imports DevExpress.CRUD.DataModel

Namespace DevExpress.CRUD.Northwind.DataModel
	Public Module NorthwindDataStorageFactory
		Public Function Create(ByVal isInDesignMode As Boolean) As NorthwindDataStorage
			If isInDesignMode Then
				Return New NorthwindDataStorage(New DesignTimeDataProvider(Of CategoryInfo)(Function(id) New CategoryInfo With {
					.Id = id,
					.Name = "Category " & id
				}), New DesignTimeDataProvider(Of ProductInfo)(Function(id) New ProductInfo With {
					.Id = id,
					.Name = "Product " & id,
					.CategoryId = id
				}))
			End If
			Return New NorthwindDataStorage(New EntityFrameworkDataProvider(Of NorthwindContext, Category, CategoryInfo)(createContext:= Function() New NorthwindContext(), getDbSet:= Function(context) context.Categories, getEnityExpression:= Function(category) New CategoryInfo With {
				.Id = category.Id,
				.Name = category.Name
			}, keyProperty:= NameOf(Category.Id)), New EntityFrameworkDataProvider(Of NorthwindContext, Product, ProductInfo)(createContext:= Function() New NorthwindContext(), getDbSet:= Function(context) context.Products, getEnityExpression:= Function(product) New ProductInfo With {
				.Id = product.Id,
				.Name = product.Name,
				.CategoryId = product.CategoryId
			}, getKey:= Function(productInfo) productInfo.Id, getEntityKey:= Function(product) product.Id, setKey:= Sub(productInfo, id) productInfo.Id = CLng(Math.Truncate(id)), applyProperties:= Sub(productInfo, product)
				product.Name = productInfo.Name
				product.CategoryId = productInfo.CategoryId
			End Sub, keyProperty:= NameOf(Category.Id)))
		End Function
	End Module
End Namespace
