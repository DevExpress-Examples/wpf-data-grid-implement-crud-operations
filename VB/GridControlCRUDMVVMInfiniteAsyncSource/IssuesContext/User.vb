Imports System.Collections.Generic

Namespace GridControlCRUDMVVMInfiniteAsyncSource
	Public Class User
		Public Property Id() As Integer
		Public Property FirstName() As String
		Public Property LastName() As String
		Public ReadOnly Property FullName() As String
			Get
				Return FirstName & " " & LastName
			End Get
		End Property
		Public Overridable Property Issues() As ICollection(Of Issue)
	End Class
End Namespace
