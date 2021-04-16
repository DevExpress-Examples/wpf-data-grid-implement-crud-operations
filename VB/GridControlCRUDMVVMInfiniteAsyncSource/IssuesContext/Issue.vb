Imports System

Namespace GridControlCRUDMVVMInfiniteAsyncSource
	Public Class Issue
		Public Property Id() As Integer
		Public Property Subject() As String
		Public Property UserId() As Integer
		Public Overridable Property User() As User
		Public Property Created() As DateTime
		Public Property Votes() As Integer
		Public Property Priority() As Priority
	End Class
	Public Enum Priority
		Low
		BelowNormal
		Normal
		AboveNormal
		High
	End Enum
End Namespace
