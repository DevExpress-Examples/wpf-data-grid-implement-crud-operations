Imports DevExpress.CRUD.ViewModel
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Data
Imports DevExpress.Xpf.Grid
Imports System
Imports System.ComponentModel
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Input

Namespace DevExpress.CRUD.View
	Public Class GridControlDeleteRefreshBehavior
		Inherits Behavior(Of TableView)

		Public Property OnDeleteCommand() As ICommand(Of RowDeleteArgs)
			Get
				Return DirectCast(GetValue(OnDeleteCommandProperty), ICommand(Of RowDeleteArgs))
			End Get
			Set(ByVal value As ICommand(Of RowDeleteArgs))
				SetValue(OnDeleteCommandProperty, value)
			End Set
		End Property
		Public Shared ReadOnly OnDeleteCommandProperty As DependencyProperty = DependencyProperty.Register("OnDeleteCommand", GetType(ICommand(Of RowDeleteArgs)), GetType(GridControlDeleteRefreshBehavior), New PropertyMetadata(Nothing))

		Public Property OnRefreshCommand() As ICommand(Of RefreshArgs)
			Get
				Return DirectCast(GetValue(OnRefreshCommandProperty), ICommand(Of RefreshArgs))
			End Get
			Set(ByVal value As ICommand(Of RefreshArgs))
				SetValue(OnRefreshCommandProperty, value)
			End Set
		End Property
		Public Shared ReadOnly OnRefreshCommandProperty As DependencyProperty = DependencyProperty.Register("OnRefreshCommand", GetType(ICommand(Of RefreshArgs)), GetType(GridControlDeleteRefreshBehavior), New PropertyMetadata(Nothing))

		Public Property NoRecordsErrorMessage() As String
			Get
				Return CStr(GetValue(NoRecordsErrorMessageProperty))
			End Get
			Set(ByVal value As String)
				SetValue(NoRecordsErrorMessageProperty, value)
			End Set
		End Property
		Public Shared ReadOnly NoRecordsErrorMessageProperty As DependencyProperty = DependencyProperty.Register("NoRecordsErrorMessage", GetType(String), GetType(GridControlDeleteRefreshBehavior), New PropertyMetadata(Nothing, Sub(d, e)
			CType(d, GridControlDeleteRefreshBehavior).UpdateErrorText()
		End Sub))


		Public ReadOnly Property DeleteCommand() As ICommand
		Public ReadOnly Property RefreshCommand() As ICommand

		Private ReadOnly Property View() As TableView
			Get
				Return AssociatedObject
			End Get
		End Property
		Private ReadOnly Property VirtualSource() As VirtualSourceBase
			Get
				Return TryCast(View?.DataControl?.ItemsSource, VirtualSourceBase)
			End Get
		End Property

		Public Sub New()
			DeleteCommand = New DelegateCommand(AddressOf DoDelete, AddressOf CanDelete)
			RefreshCommand = New AsyncCommand(AddressOf DoRefresh, AddressOf CanRefresh)
		End Sub

		Protected Overrides Sub OnAttached()
			MyBase.OnAttached()
			AddHandler View.PreviewKeyDown, AddressOf OnPreviewKeyDown
			UpdateErrorText()
		End Sub

		Protected Overrides Sub OnDetaching()
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
				e.Handled = True
				DoDelete()
			End If
			If e.Key = Key.F5 AndAlso CanRefresh() Then
				e.Handled = True
				Await DoRefresh()
			End If
		End Sub

		Private isRefreshInProgress As Boolean
		Private Async Function DoRefresh() As Task
			VirtualSource?.RefreshRows()
			Dim args = New RefreshArgs()
			OnRefreshCommand.Execute(args)
			If args.Result IsNot Nothing Then
				isRefreshInProgress = True
				Try
					Await args.Result
				Finally
					isRefreshInProgress = False
				End Try
			End If
		End Function
		Private Function CanRefresh() As Boolean
			Dim canRefreshVirtualSource = VirtualSource Is Nothing OrElse (TryCast(VirtualSource, InfiniteAsyncSource)?.IsResetting <> True AndAlso Not VirtualSource.AreRowsFetching)
			Return canRefreshVirtualSource AndAlso OnRefreshCommand IsNot Nothing AndAlso Not IsEditingRowState() AndAlso Not isRefreshInProgress AndAlso (View?.Grid.ItemsSource IsNot Nothing OrElse NoRecordsErrorMessage IsNot Nothing)
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
				OnDeleteCommand.Execute(New RowDeleteArgs(row))
				If VirtualSource IsNot Nothing Then
					VirtualSource?.RefreshRows()
				Else
					View.Commands.DeleteFocusedRow.Execute(Nothing)
				End If
			Catch ex As Exception
				DXMessageBox.Show(ex.Message)
			End Try
		End Sub

		Private Function CanDelete() As Boolean
			Return OnDeleteCommand IsNot Nothing AndAlso Not IsEditingRowState() AndAlso Not isRefreshInProgress AndAlso View?.Grid.CurrentItem IsNot Nothing
		End Function

		Private Function IsEditingRowState() As Boolean
			Return View?.AreUpdateRowButtonsShown = True
		End Function
	End Class
End Namespace
