Imports DevExpress.CRUD.DataModel

Namespace DevExpress.CRUD.Northwind.DataModel
	Public Class NorthwindDataStorage
'INSTANT VB NOTE: The variable categories was renamed since Visual Basic does not handle local variables named the same as class members well:
'INSTANT VB NOTE: The variable products was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(ByVal categories_Conflict As IDataProvider(Of CategoryInfo), ByVal products_Conflict As ICRUDDataProvider(Of ProductInfo))
			Me.Categories = categories_Conflict
			Me.Products = products_Conflict
		End Sub
		Public ReadOnly Property Categories() As IDataProvider(Of CategoryInfo)
		Public ReadOnly Property Products() As ICRUDDataProvider(Of ProductInfo)
	End Class
End Namespace
