Imports DevExpress.CRUD.DataModel
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Xpf.Data
Imports DevExpress.Mvvm.Xpf
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Threading.Tasks
Imports System.ComponentModel
Imports DevExpress.Xpf.Core

Namespace DevExpress.CRUD.ViewModel
	Public MustInherit Class VirtualCollectionViewModel(Of T As {Class, New})
		Inherits ViewModelBase

		Private ReadOnly dataProvider As IDataProvider(Of T)

		Protected Sub New(ByVal dataProvider As IDataProvider(Of T))
			Me.dataProvider = dataProvider
			StartRefresh()
		End Sub

		Public ReadOnly Property FilterType() As Type
			Get
				Return GetType(Expression(Of Func(Of T, Boolean)))
			End Get
		End Property
		Private ReadOnly Property DialogService() As IDialogService
			Get
				Return GetService(Of IDialogService)()
			End Get
		End Property

		<Command>
		Public Sub Fetch(ByVal args As FetchRowsAsyncArgs)
			args.Result = dataProvider.GetQueryableResultAsync(Of T, FetchRowsResult)(Function(queryable)
				Return queryable.SortBy(args.SortOrder, defaultUniqueSortPropertyName:= dataProvider.KeyProperty).Where(CType(args.Filter, Expression(Of Func(Of T, Boolean)))).Skip(args.Skip).Take(If(args.Take, 30)).ToArray()
			End Function)
		End Sub

		<Command>
		Public Sub GetTotalSummaries(ByVal args As GetSummariesAsyncArgs)
			args.Result = dataProvider.GetQueryableResultAsync(Function(queryable)
				Return queryable.Where(CType(args.Filter, Expression(Of Func(Of T, Boolean)))).GetSummaries(args.Summaries)
			End Function)
		End Sub

		<Command>
		Public Sub GetUniqueValues(ByVal args As GetUniqueValuesAsyncArgs)
			args.ResultWithCounts = dataProvider.GetQueryableResultAsync(Function(queryable)
				Return queryable.Where(CType(args.Filter, Expression(Of Func(Of T, Boolean)))).DistinctWithCounts(args.PropertyName)
			End Function)
		End Sub

		<Command>
		Public Sub OnUpdate(ByVal args As EntityUpdateArgs)
			Dim entity = DirectCast(args.Entity, T)
			Dim commands = CreateCommands(Sub()
				dataProvider.Update(entity)
				args.Updated = True
			End Sub)
			DialogService.ShowDialog(commands, "Edit " & GetType(T).Name, CreateEntityViewModel(entity))
		End Sub

		<Command>
		Public Sub OnCreate(ByVal args As EntityCreateArgs)
			Dim entity = New T()
			Dim commands = CreateCommands(Sub()
				dataProvider.Create(entity)
				args.Entity = entity
			End Sub)
			DialogService.ShowDialog(commands, "New " & GetType(T).Name, CreateEntityViewModel(entity))
		End Sub

		Private Function CreateCommands(ByVal saveAction As Action) As UICommand()
			Return { New UICommand(Nothing, "Save", New DelegateCommand(Of CancelEventArgs)(Function(cancelArgs)
				Try
					saveAction()
				Catch e As Exception
					GetService(Of IMessageBoxService)().ShowMessage(e.Message, "Error", MessageButton.OK)
					cancelArgs.Cancel = True
				End Try
			End Function), isDefault:= True, isCancel:= False), New UICommand(Nothing, "Cancel", Nothing, isDefault:= False, isCancel:= True)}
		End Function

		Protected MustOverride Function CreateEntityViewModel(ByVal entity As T) As EntityViewModel(Of T)

		<Command>
		Public Sub OnDelete(ByVal args As RowDeleteArgs)
			Me.dataProvider.Delete(DirectCast(args.Row, T))
		End Sub

		<Command>
		Public Sub OnRefresh(ByVal args As RefreshArgs)
			args.Result = OnRefreshCoreAsync()
		End Sub
		Private Async Sub StartRefresh()
			Await OnRefreshCoreAsync()
		End Sub
		Protected Overridable Function OnRefreshCoreAsync() As Task
			Return Task.CompletedTask
		End Function
	End Class
	Public Class EntityViewModel(Of T)
		Inherits ViewModelBase

		Public Sub New(ByVal entity As T)
			Me.Entity = entity
		End Sub
		Public ReadOnly Property Entity() As T
	End Class
End Namespace
