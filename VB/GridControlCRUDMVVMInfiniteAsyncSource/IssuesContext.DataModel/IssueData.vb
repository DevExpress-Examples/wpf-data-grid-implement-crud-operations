Imports System
Imports System.Linq

Namespace GridControlCRUDMVVMInfiniteAsyncSource
	Public Class IssueData
		Public Sub New()
			Created = DateTime.Today
		End Sub
		Public Property Id() As Integer
		Public Property Subject() As String
		Public Property Created() As DateTime
		Public Property Votes() As Integer
		Public Property Priority() As Priority
		Public Property UserId() As Integer
	End Class
End Namespace
