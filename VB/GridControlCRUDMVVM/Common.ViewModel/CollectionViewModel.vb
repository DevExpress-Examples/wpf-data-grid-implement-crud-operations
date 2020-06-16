Imports DevExpress.CRUD.DataModel
Imports DevExpress.Mvvm
Imports System.Collections.Generic
Imports System.Windows.Input

Namespace DevExpress.CRUD.ViewModel
	Public MustInherit Class CollectionViewModel(Of T As Class)
		Inherits ViewModelBase

		Private ReadOnly dataProvider As ICRUDDataProvider(Of T)

		Protected Sub New(ByVal dataProvider As ICRUDDataProvider(Of T))
			Me.dataProvider = dataProvider
			OnRefresh()
			OnRefreshCommand = New DelegateCommand(AddressOf OnRefresh)
			OnCreateCommand = New DelegateCommand(Of T)(AddressOf Me.dataProvider.Create)
			OnUpdateCommand = New DelegateCommand(Of T)(AddressOf Me.dataProvider.Update)
			OnDeleteCommand = New DelegateCommand(Of T)(AddressOf Me.dataProvider.Delete)
		End Sub

		Public ReadOnly Property OnRefreshCommand() As ICommand
		Public ReadOnly Property OnCreateCommand() As ICommand(Of T)
		Public ReadOnly Property OnUpdateCommand() As ICommand(Of T)
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

		Private Sub OnRefresh()
			Try
				Entities = dataProvider.Read()
				EntitiesErrorMessage = Nothing
			Catch
				Entities = Nothing
				EntitiesErrorMessage = "An error has occurred while establishing a connection to the database. Press F5 to retry the connection."
			End Try
			OnRefreshCore()
		End Sub
		Protected Overridable Sub OnRefreshCore()
		End Sub
	End Class
End Namespace
