Imports System
Imports DevExpress.CRUD.DataModel.EntityFramework
Imports DevExpress.CRUD.DataModel

Namespace GridControlCRUDMVVMInfiniteAsyncSource
	Public Module IssuesDataStorageFactory
		Public Function Create(ByVal isInDesignMode As Boolean) As IssuesDataStorage
			If isInDesignMode Then
				Return New IssuesDataStorage(New DesignTimeDataProvider(Of IssueData)(Function(id) New IssueData With {
					.Id = id,
					.Subject = "Subject " & id
				}), New DesignTimeDataProvider(Of User)(Function(id) New User With {
					.Id = id,
					.FirstName = "FirstName " & id
				}))
			End If
			Return New IssuesDataStorage(New EntityFrameworkDataProvider(Of IssuesContext, Issue, IssueData)(createContext:= Function() New IssuesContext(), getDbSet:= Function(context) context.Issues, getEnityExpression:= Function(x) New IssueData() With {
				.Id = x.Id,
				.Subject = x.Subject,
				.UserId = x.UserId,
				.Created = x.Created,
				.Votes = x.Votes,
				.Priority = x.Priority
			}, getKey:= Function(ussueData) ussueData.Id, getEntityKey:= Function(ussue) ussue.Id, setKey:= Sub(ussueData, id) ussueData.Id = CInt(Math.Truncate(id)), applyProperties:= Sub(ussueData, issue)
				issue.Subject = ussueData.Subject
				issue.UserId = ussueData.UserId
				issue.Created = ussueData.Created
				issue.Votes = ussueData.Votes
				issue.Priority = ussueData.Priority
			End Sub, keyProperty:= NameOf(IssueData.Id)), New EntityFrameworkDataProvider<IssuesContext, User, User>(createContext:= Function() New IssuesContext(), getDbSet:= Function(context) context.Users, getEnityExpression:= Function(user) user, keyProperty:= NameOf(User.Id)))
		End Function
	End Module
End Namespace
