Imports DevExpress.CRUD.ViewModel
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Data
Imports DevExpress.Xpf.Grid
Imports System
Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Input

Namespace GridControlCRUDMVVMInfiniteAsyncSource
	Public Class VirtualSourceEditFormBehavior
		Inherits Behavior(Of TableView)

		Public Property OnUpdateCommand() As ICommand
			Get
				Return DirectCast(GetValue(OnUpdateCommandProperty), ICommand)
			End Get
			Set(ByVal value As ICommand)
				SetValue(OnUpdateCommandProperty, value)
			End Set
		End Property
		Public Shared ReadOnly OnUpdateCommandProperty As DependencyProperty = DependencyProperty.Register("OnUpdateCommand", GetType(ICommand), GetType(VirtualSourceEditFormBehavior), New PropertyMetadata(Nothing))

		Public Property OnCreateCommand() As ICommand
			Get
				Return DirectCast(GetValue(OnCreateCommandProperty), ICommand)
			End Get
			Set(ByVal value As ICommand)
				SetValue(OnCreateCommandProperty, value)
			End Set
		End Property
		Public Shared ReadOnly OnCreateCommandProperty As DependencyProperty = DependencyProperty.Register("OnCreateCommand", GetType(ICommand), GetType(VirtualSourceEditFormBehavior), New PropertyMetadata(Nothing))

		Private ReadOnly Property Source() As VirtualSourceBase
			Get
				Return CType(AssociatedObject?.DataControl?.ItemsSource, VirtualSourceBase)
			End Get
		End Property

		Public ReadOnly Property CreateCommand() As ICommand
		Public ReadOnly Property UpdateCommand() As ICommand
		Public Sub New()
			CreateCommand = New DelegateCommand(AddressOf DoCreate)
			UpdateCommand = New DelegateCommand(Sub() DoUpdate(), AddressOf CanUpdate)
		End Sub

		Protected Overrides Sub OnAttached()
			MyBase.OnAttached()
			AddHandler AssociatedObject.PreviewKeyDown, AddressOf OnKeyDown
			AddHandler AssociatedObject.MouseDoubleClick, AddressOf OnMouseDoubleClick
		End Sub

		Protected Overrides Sub OnDetaching()
			RemoveHandler AssociatedObject.PreviewKeyDown, AddressOf OnKeyDown
			RemoveHandler AssociatedObject.MouseDoubleClick, AddressOf OnMouseDoubleClick
			MyBase.OnDetaching()
		End Sub

		Private Sub OnMouseDoubleClick(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			Dim row = EventArgsToDataRowConverter.GetDataRow(e)
			If row IsNot Nothing Then
				DoUpdate(row)
			End If
		End Sub

		Private Sub OnKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
			If e.Key = Key.F2 Then
				DoUpdate()
				e.Handled = True
			End If
			If e.Key = Key.N AndAlso (Keyboard.Modifiers And ModifierKeys.Control) <> 0 Then
				DoCreate()
				e.Handled = True
			End If
		End Sub

		Private Sub DoUpdate(Optional ByVal entity As Object = Nothing)
			entity = If(entity, AssociatedObject.DataControl.CurrentItem)
			Dim args = New EntityUpdateArgs(entity)
			OnUpdateCommand.Execute(args)
			If args.Updated Then
				ReloadRow(GetKey(entity))
			End If
		End Sub
		Private Sub ReloadRow(ByVal key As Object)
			If TypeOf Source Is InfiniteAsyncSource Then
				CType(Source, InfiniteAsyncSource).ReloadRows(key)
			ElseIf TypeOf Source Is PagedAsyncSource Then
				CType(Source, PagedAsyncSource).ReloadRows(key)
			Else
				Throw New InvalidOperationException()
			End If
		End Sub

		Private Function CanUpdate() As Boolean
			Return OnUpdateCommand IsNot Nothing AndAlso CanChangeCurrentItem()
		End Function

		Private Function CanChangeCurrentItem() As Boolean
			Return AssociatedObject?.DataControl?.CurrentItem IsNot Nothing
		End Function

		Private Sub DoCreate()
			Dim args = New EntityCreateArgs()
			OnCreateCommand.Execute(args)
			If args.Entity IsNot Nothing Then
				Source.RefreshRows()
			End If
		End Sub

		Private Function GetKey(Of T)(ByVal entity As T) As Object
			Dim typedList As ITypedList = Source
			Dim keyProperty = typedList.GetItemProperties(Nothing)(Source.KeyProperty)
			Return keyProperty.GetValue(entity)
		End Function
	End Class
End Namespace
