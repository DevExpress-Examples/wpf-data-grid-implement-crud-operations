Imports DevExpress.CRUD.DataModel
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Mvvm.Xpf
Imports System.Collections.Generic

Namespace DevExpress.CRUD.ViewModel
	Public MustInherit Class CollectionViewModel(Of T As Class)
		Inherits ViewModelBase

		Private ReadOnly dataProvider As IDataProvider(Of T)

		Protected Sub New(ByVal dataProvider As IDataProvider(Of T))
			Me.dataProvider = dataProvider
			OnRefresh()
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

'INSTANT VB NOTE: An underscore by itself is not a valid identifier in VB:
'ORIGINAL LINE: public void OnRefresh(RefreshArgs _)
		<Command>
		Public Sub OnRefresh(ByVal underscore As RefreshArgs)
			OnRefresh()
		End Sub
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

		<Command>
		Public Sub OnUpdateRow(ByVal args As RowValidationArgs)
			Dim entity = CType(args.Item, T)
			If args.IsNewItem Then
				dataProvider.Create(entity)
			Else
				dataProvider.Update(entity)
			End If
		End Sub

		<Command>
		Public Sub OnDelete(ByVal args As RowDeleteArgs)
			dataProvider.Delete(DirectCast(args.Row, T))
		End Sub
	End Class
End Namespace
