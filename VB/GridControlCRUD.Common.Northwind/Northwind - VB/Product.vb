Imports System
Imports System.Linq

Namespace DevExpress.CRUD.Northwind
	Public Class Product
		Public Property Id() As Long
		Public Property Name() As String
		Public Property CategoryId() As Long
		Public Overridable Property Category() As Category
		Public Property QuantityPerUnit() As String
		Public Property UnitPrice() As Decimal?
		Public Property UnitsInStock() As Short?
		Public Property UnitsOnOrder() As Short?
		Public Property ReorderLevel() As Short?
		Public Property Discontinued() As Boolean
		Public Property EAN13() As String
	End Class
End Namespace
