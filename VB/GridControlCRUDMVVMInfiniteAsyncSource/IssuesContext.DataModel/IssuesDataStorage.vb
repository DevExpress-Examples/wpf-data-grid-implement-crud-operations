Imports DevExpress.CRUD.DataModel

Namespace GridControlCRUDMVVMInfiniteAsyncSource
	Public Class IssuesDataStorage
		Public Sub New(ByVal issues As IDataProvider(Of IssueData), ByVal users As IDataProvider(Of User))
			Me.Users = users
			Me.Issues = issues
		End Sub
		Public ReadOnly Property Users() As IDataProvider(Of User)
		Public ReadOnly Property Issues() As IDataProvider(Of IssueData)
	End Class
End Namespace
