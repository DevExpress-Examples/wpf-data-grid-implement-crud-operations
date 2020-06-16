Imports DevExpress.CRUD.DataModel
Imports DevExpress.Mvvm
Imports System.Collections.Generic
Imports System.Threading.Tasks

Namespace DevExpress.CRUD.ViewModel
	Public MustInherit Class CollectionViewModel(Of T As Class)
		Inherits ViewModelBase

		Private ReadOnly dataProvider As ICRUDDataProvider(Of T)

		Protected Sub New(ByVal dataProvider As ICRUDDataProvider(Of T))
			Me.dataProvider = dataProvider
			StartRefresh()
			OnRefreshCommand = New AsyncCommand(AddressOf OnRefreshAsync)
			OnCreateCommand = New AsyncCommand(Of Object)(Function(entity) Me.dataProvider.CreateAsync(CType(entity, T)))
			OnUpdateCommand = New AsyncCommand(Of Object)(Function(entity) Me.dataProvider.UpdateAsync(CType(entity, T)))
			OnDeleteCommand = New DelegateCommand(Of T)(AddressOf Me.dataProvider.Delete)
		End Sub

		Public ReadOnly Property OnRefreshCommand() As AsyncCommand
		Public ReadOnly Property OnCreateCommand() As AsyncCommand(Of Object)
		Public ReadOnly Property OnUpdateCommand() As AsyncCommand(Of Object)
		Public ReadOnly Property OnDeleteCommand() As ICommand(Of T)

		Public Property Entities() As IList(Of T)
			Get
				Return GetValue(Of IList(Of T))()
			End Get
			Private Set(ByVal value As IList(Of T))
				SetValue(value)
			End Set
		End Property
		Public Property EntitiesErrorMessage() As String
			Get
				Return GetValue(Of String)()
			End Get
			Private Set(ByVal value As String)
				SetValue(value)
			End Set
		End Property
		Public Property IsLoading() As Boolean
			Get
				Return GetValue(Of Boolean)()
			End Get
			Private Set(ByVal value As Boolean)
				SetValue(value)
			End Set
		End Property

		Private Async Sub StartRefresh()
			Await OnRefreshAsync()
		End Sub

		Private Async Function OnRefreshAsync() As Task
			IsLoading = True
			Try
				Await Task.WhenAll(RefreshEntities(), OnRefreshCoreAsync())
			Finally
				IsLoading = False
			End Try
		End Function
		Private Async Function RefreshEntities() As Task
			Try
				Entities = Await dataProvider.ReadAsync()
				EntitiesErrorMessage = Nothing
			Catch
				Entities = Nothing
				EntitiesErrorMessage = "An error has occurred while establishing a connection to the database. Press F5 to retry the connection."
			End Try
		End Function
		Protected Overridable Function OnRefreshCoreAsync() As Task
			Return Task.CompletedTask
		End Function
	End Class
End Namespace
