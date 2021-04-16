Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace DevExpress.CRUD.Northwind
	Public Class Category
		Public Property Id() As Long
		Public Property Name() As String
		Public Property Description() As String
		Public Overridable Property Products() As ICollection(Of Product)
	End Class
End Namespace
