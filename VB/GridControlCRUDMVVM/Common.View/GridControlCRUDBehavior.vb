Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Grid
Imports System
Imports System.Windows
Imports System.Windows.Input

Namespace DevExpress.CRUD.View
	Public Class GridControlCRUDBehavior
		Inherits Behavior(Of TableView)

		Public Property OnCreateCommand() As ICommand
			Get
				Return DirectCast(GetValue(OnCreateCommandProperty), ICommand)
			End Get
			Set(ByVal value As ICommand)
				SetValue(OnCreateCommandProperty, value)
			End Set
		End Property
		Public Shared ReadOnly OnCreateCommandProperty As DependencyProperty = DependencyProperty.Register("OnCreateCommand", GetType(ICommand), GetType(GridControlCRUDBehavior), New PropertyMetadata(Nothing))

		Public Property OnUpdateCommand() As ICommand
			Get
				Return DirectCast(GetValue(OnUpdateCommandProperty), ICommand)
			End Get
			Set(ByVal value As ICommand)
				SetValue(OnUpdateCommandProperty, value)
			End Set
		End Property
		Public Shared ReadOnly OnUpdateCommandProperty As DependencyProperty = DependencyProperty.Register("OnUpdateCommand", GetType(ICommand), GetType(GridControlCRUDBehavior), New PropertyMetadata(Nothing))

		Public Property OnDeleteCommand() As ICommand
			Get
				Return DirectCast(GetValue(OnDeleteCommandProperty), ICommand)
			End Get
			Set(ByVal value As ICommand)
				SetValue(OnDeleteCommandProperty, value)
			End Set
		End Property
		Public Shared ReadOnly OnDeleteCommandProperty As DependencyProperty = DependencyProperty.Register("OnDeleteCommand", GetType(ICommand), GetType(GridControlCRUDBehavior), New PropertyMetadata(Nothing))

		Public Property OnRefreshCommand() As ICommand
			Get
				Return DirectCast(GetValue(OnRefreshCommandProperty), ICommand)
			End Get
			Set(ByVal value As ICommand)
				SetValue(OnRefreshCommandProperty, value)
			End Set
		End Property
		Public Shared ReadOnly OnRefreshCommandProperty As DependencyProperty = DependencyProperty.Register("OnRefreshCommand", GetType(ICommand), GetType(GridControlCRUDBehavior), New PropertyMetadata(Nothing))

		Public Property NoRecordsErrorMessage() As String
			Get
				Return CStr(GetValue(NoRecordsErrorMessageProperty))
			End Get
			Set(ByVal value As String)
				SetValue(NoRecordsErrorMessageProperty, value)
			End Set
		End Property
		Public Shared ReadOnly NoRecordsErrorMessageProperty As DependencyProperty = DependencyProperty.Register("NoRecordsErrorMessage", GetType(String), GetType(GridControlCRUDBehavior), New PropertyMetadata(Nothing, Sub(d, e)
			CType(d, GridControlCRUDBehavior).UpdateErrorText()
		End Sub))

		Public ReadOnly Property DeleteCommand() As ICommand
		Public ReadOnly Property RefreshCommand() As ICommand

		Private ReadOnly Property View() As TableView
			Get
				Return AssociatedObject
			End Get
		End Property

		Public Sub New()
			DeleteCommand = New DelegateCommand(AddressOf DoDelete, AddressOf CanDelete)
			RefreshCommand = New DelegateCommand(AddressOf DoRefresh, AddressOf CanRefresh)
		End Sub

		Protected Overrides Sub OnAttached()
			MyBase.OnAttached()
			AddHandler View.ValidateRow, AddressOf OnValidateRow
			AddHandler View.PreviewKeyDown, AddressOf OnPreviewKeyDown
			UpdateErrorText()
		End Sub

		Protected Overrides Sub OnDetaching()
			RemoveHandler View.ValidateRow, AddressOf OnValidateRow
			RemoveHandler View.PreviewKeyDown, AddressOf OnPreviewKeyDown
			UpdateErrorText()
			MyBase.OnDetaching()
		End Sub

		Private Sub UpdateErrorText()
			If View Is Nothing Then
				Return
			End If
			If NoRecordsErrorMessage IsNot Nothing Then
				View.ShowEmptyText = True
				View.RuntimeLocalizationStrings = New GridRuntimeStringCollection() From {New RuntimeStringIdInfo(GridControlRuntimeStringId.NoRecords, NoRecordsErrorMessage)}
			Else
				View.ShowEmptyText = False
				View.RuntimeLocalizationStrings = Nothing
			End If
		End Sub

		Private Sub OnPreviewKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
			If e.Key = Key.Delete AndAlso CanDelete() Then
				DoDelete()
				e.Handled = True
			End If
			If e.Key = Key.F5 AndAlso CanRefresh() Then
				DoRefresh()
				e.Handled = True
			End If
		End Sub

		Private Sub DoRefresh()
			OnRefreshCommand.Execute(Nothing)
		End Sub
		Private Function CanRefresh() As Boolean
			Return OnRefreshCommand IsNot Nothing AndAlso Not IsEditingRowState()
		End Function

		Private Sub DoDelete()
			Dim row = View.Grid.SelectedItem
			If row Is Nothing Then
				Return
			End If
			If DXMessageBox.Show(View, "Are you sure you want to delete this row?", "Delete Row", MessageBoxButton.OKCancel) = MessageBoxResult.Cancel Then
				Return
			End If
			Try
				OnDeleteCommand.Execute(row)
				View.Commands.DeleteFocusedRow.Execute(Nothing)
			Catch ex As Exception
				DXMessageBox.Show(ex.Message)
			End Try
		End Sub

		Private Function CanDelete() As Boolean
			Return OnDeleteCommand IsNot Nothing AndAlso Not IsEditingRowState()
		End Function

		Private Function IsEditingRowState() As Boolean
			Return View?.AreUpdateRowButtonsShown = True
		End Function

		Private Sub OnValidateRow(ByVal sender As Object, ByVal e As GridRowValidationEventArgs)
			If View.FocusedRowHandle = DataControlBase.NewItemRowHandle Then
				OnCreateCommand.Execute(e.Row)
			Else
				OnUpdateCommand.Execute(e.Row)
			End If
		End Sub
	End Class
End Namespace
