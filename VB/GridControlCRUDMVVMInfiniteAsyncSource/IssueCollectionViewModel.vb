Imports DevExpress.CRUD.DataModel
Imports DevExpress.CRUD.ViewModel
Imports GridControlCRUDMVVMInfiniteAsyncSource
Imports System.Collections.Generic
Imports System.Threading.Tasks

Namespace GridControlCRUDMVVMInfiniteAsyncSource
	Public Class IssueCollectionViewModel
		Inherits VirtualCollectionViewModel(Of IssueData)

		Public Property Users() As IList(Of User)
			Get
				Return GetValue(Of IList(Of User))()
			End Get
			Private Set(ByVal value As IList(Of User))
				SetValue(value)
			End Set
		End Property

		Private ReadOnly usersDataProvider As IDataProvider(Of User)

		Public Sub New()
			Me.New(IssuesDataStorageFactory.Create(IsInDesignMode))
		End Sub

		Public Sub New(ByVal dataStorage As IssuesDataStorage)
			MyBase.New(dataStorage.Issues)
			usersDataProvider = dataStorage.Users
			RefreshUsers()
		End Sub

		Private Async Sub RefreshUsers()
			Await OnRefreshCoreAsync()
		End Sub
		Protected Overrides Async Function OnRefreshCoreAsync() As Task
			If usersDataProvider IsNot Nothing Then
				Try
					Users = Await usersDataProvider.ReadAsync()
				Catch
					Users = Nothing
				End Try
			End If
		End Function
		Protected Overrides Function CreateEntityViewModel(ByVal entity As IssueData) As EntityViewModel(Of IssueData)
			Return New IssueDataViewModel(entity, usersDataProvider.ReadAsync())
		End Function
	End Class
	Public Class IssueDataViewModel
		Inherits EntityViewModel(Of IssueData)

		Public Sub New(ByVal entity As IssueData, ByVal usersTask As Task(Of IList(Of User)))
			MyBase.New(entity)
			AssignUsers(usersTask)
		End Sub
		Private Async Sub AssignUsers(ByVal usersTask As Task(Of IList(Of User)))
			Try
				Users = Await usersTask
			Catch
				Users = Nothing
			End Try
		End Sub
		Public Property Users() As IList(Of User)
			Get
				Return GetValue(Of IList(Of User))()
			End Get
			Private Set(ByVal value As IList(Of User))
				SetValue(value)
			End Set
		End Property
	End Class
End Namespace
