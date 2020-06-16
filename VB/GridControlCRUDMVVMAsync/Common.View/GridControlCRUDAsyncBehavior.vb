Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Grid
Imports System
Imports System.ComponentModel
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Input

Namespace DevExpress.CRUD.View
	Public Class GridControlCRUDAsyncBehavior
		Inherits Behavior(Of TableView)

		Public Property OnCreateCommand() As AsyncCommand(Of Object)
			Get
				Return CType(GetValue(OnCreateCommandProperty), AsyncCommand(Of Object))
			End Get
			Set(ByVal value As AsyncCommand(Of Object))
				SetValue(OnCreateCommandProperty, value)
			End Set
		End Property
		Public Shared ReadOnly OnCreateCommandProperty As DependencyProperty = DependencyProperty.Register("OnCreateCommand", GetType(AsyncCommand(Of Object)), GetType(GridControlCRUDAsyncBehavior), New PropertyMetadata(Nothing))

		Public Property OnUpdateCommand() As AsyncCommand(Of Object)
			Get
				Return CType(GetValue(OnUpdateCommandProperty), AsyncCommand(Of Object))
			End Get
			Set(ByVal value As AsyncCommand(Of Object))
				SetValue(OnUpdateCommandProperty, value)
			End Set
		End Property
		Public Shared ReadOnly OnUpdateCommandProperty As DependencyProperty = DependencyProperty.Register("OnUpdateCommand", GetType(AsyncCommand(Of Object)), GetType(GridControlCRUDAsyncBehavior), New PropertyMetadata(Nothing))

		Public Property OnDeleteCommand() As ICommand
			Get
				Return DirectCast(GetValue(OnDeleteCommandProperty), ICommand)
			End Get
			Set(ByVal value As ICommand)
				SetValue(OnDeleteCommandProperty, value)
			End Set
		End Property
		Public Shared ReadOnly OnDeleteCommandProperty As DependencyProperty = DependencyProperty.Register("OnDeleteCommand", GetType(ICommand), GetType(GridControlCRUDAsyncBehavior), New PropertyMetadata(Nothing))

		Public Property OnRefreshCommand() As AsyncCommand
			Get
				Return CType(GetValue(OnRefreshCommandProperty), AsyncCommand)
			End Get
			Set(ByVal value As AsyncCommand)
				SetValue(OnRefreshCommandProperty, value)
			End Set
		End Property
		Public Shared ReadOnly OnRefreshCommandProperty As DependencyProperty = DependencyProperty.Register("OnRefreshCommand", GetType(AsyncCommand), GetType(GridControlCRUDAsyncBehavior), New PropertyMetadata(Nothing))

		Public Property NoRecordsErrorMessage() As String
			Get
				Return CStr(GetValue(NoRecordsErrorMessageProperty))
			End Get
			Set(ByVal value As String)
				SetValue(NoRecordsErrorMessageProperty, value)
			End Set
		End Property
		Public Shared ReadOnly NoRecordsErrorMessageProperty As DependencyProperty = DependencyProperty.Register("NoRecordsErrorMessage", GetType(String), GetType(GridControlCRUDAsyncBehavior), New PropertyMetadata(Nothing, Sub(d, e) (CType(d, GridControlCRUDAsyncBehavior)).UpdateErrorText()))


		Public ReadOnly Property DeleteCommand() As ICommand
		Public ReadOnly Property RefreshCommand() As ICommand

		Private ReadOnly Property View() As TableView
			Get
				Return AssociatedObject
			End Get
		End Property

		Public Sub New()
			DeleteCommand = New DelegateCommand(AddressOf DoDelete, AddressOf CanDelete)
			RefreshCommand = New AsyncCommand(AddressOf DoRefresh, AddressOf CanRefresh)
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

		Private Async Sub OnPreviewKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
			If e.Key = Key.Delete AndAlso CanDelete() Then
				DoDelete()
				e.Handled = True
			End If
			If e.Key = Key.F5 AndAlso CanRefresh() Then
				Await DoRefresh()
				e.Handled = True
			End If
		End Sub

		Private Function DoRefresh() As Task
			Return OnRefreshCommand.ExecuteAsync(Nothing)
		End Function
		Private Function CanRefresh() As Boolean
			Return OnRefreshCommand IsNot Nothing AndAlso Not IsEditingRowState() AndAlso Not OnRefreshCommand.IsExecuting AndAlso (View?.Grid.ItemsSource IsNot Nothing OrElse NoRecordsErrorMessage IsNot Nothing)
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
			Return OnDeleteCommand IsNot Nothing AndAlso Not IsEditingRowState() AndAlso Not OnRefreshCommand.IsExecuting AndAlso View?.Grid.CurrentItem IsNot Nothing
		End Function

		Private Function IsEditingRowState() As Boolean
			Return View?.AreUpdateRowButtonsShown = True
		End Function

		Private Sub OnValidateRow(ByVal sender As Object, ByVal e As GridRowValidationEventArgs)
			If View.FocusedRowHandle = DataControlBase.NewItemRowHandle Then
				e.UpdateRowResult = OnCreateCommand.ExecuteAsync(e.Row)
			Else
				e.UpdateRowResult = OnUpdateCommand.ExecuteAsync(e.Row)
			End If
		End Sub
	End Class
End Namespace
