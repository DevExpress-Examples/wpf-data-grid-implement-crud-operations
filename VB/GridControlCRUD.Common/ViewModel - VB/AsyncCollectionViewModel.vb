Imports DevExpress.CRUD.DataModel
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Mvvm.Xpf
Imports System.Collections.Generic
Imports System.Threading.Tasks

Namespace DevExpress.CRUD.ViewModel
	Public MustInherit Class AsyncCollectionViewModel(Of T As Class)
		Inherits ViewModelBase

		Private ReadOnly dataProvider As IDataProvider(Of T)

		Protected Sub New(ByVal dataProvider As IDataProvider(Of T))
			Me.dataProvider = dataProvider
			StartRefresh()
		End Sub

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

		<Command>
		Public Sub OnRefresh(ByVal args As RefreshArgs)
			args.Result = OnRefreshAsync()
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

		<Command>
		Public Sub OnUpdateRow(ByVal args As RowValidationArgs)
			args.ResultAsync = OnUpdateRowAsync(CType(args.Item, T), args.IsNewItem)
		End Sub
		Private Async Function OnUpdateRowAsync(ByVal entity As T, ByVal isNew As Boolean) As Task(Of ValidationErrorInfo)
			If isNew Then
				Await dataProvider.CreateAsync(entity)
			Else
				Await dataProvider.UpdateAsync(entity)
			End If
			Return Nothing
		End Function

		<Command>
		Public Sub OnDelete(ByVal args As RowDeleteArgs)
			dataProvider.Delete(DirectCast(args.Row, T))
		End Sub
	End Class
End Namespace
