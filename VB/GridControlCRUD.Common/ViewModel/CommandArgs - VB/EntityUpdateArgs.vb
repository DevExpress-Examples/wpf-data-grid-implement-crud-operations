Imports System
Imports System.Linq

Namespace DevExpress.CRUD.ViewModel
	Public Class EntityUpdateArgs
		Public Sub New(ByVal entity As Object)
			Me.Entity = entity
		End Sub
		Public ReadOnly Property Entity() As Object
		Public Property Updated() As Boolean
	End Class
End Namespace
