Imports DevExpress.CRUD.DataModel

Namespace DevExpress.CRUD.Northwind.DataModel
	Public Class NorthwindDataStorage
		Public Sub New(ByVal categories As IDataProvider(Of CategoryInfo), ByVal products As IDataProvider(Of ProductInfo))
			Me.Categories = categories
			Me.Products = products
		End Sub
		Public ReadOnly Property Categories() As IDataProvider(Of CategoryInfo)
		Public ReadOnly Property Products() As IDataProvider(Of ProductInfo)
	End Class
End Namespace
