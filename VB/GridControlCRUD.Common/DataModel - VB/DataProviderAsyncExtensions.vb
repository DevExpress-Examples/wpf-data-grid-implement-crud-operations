Imports DevExpress.CRUD.DataModel
Imports DevExpress.Xpf.Data
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Threading.Tasks

Namespace DevExpress.CRUD.ViewModel
	Public Module DataProviderAsyncExtensions
		<System.Runtime.CompilerServices.Extension> _
		Public Async Function ReadAsync(Of T As Class)(ByVal dataProvider As IDataProvider(Of T)) As Task(Of IList(Of T))
#If DEBUG Then
			Await Task.Delay(500)
#End If
			Return Await Task.Run(Function() dataProvider.Read())
		End Function
		<System.Runtime.CompilerServices.Extension> _
		Public Async Function GetQueryableResultAsync(Of T As Class, TResult)(ByVal dataProvider As IDataProvider(Of T), ByVal getResult As Func(Of IQueryable(Of T), TResult)) As Task(Of TResult)
#If DEBUG Then
			Await Task.Delay(500)
#End If
			Return Await Task.Run(Function() dataProvider.GetQueryableResult(getResult))
		End Function
		<System.Runtime.CompilerServices.Extension> _
		Public Async Function UpdateAsync(Of T As Class)(ByVal dataProvider As IDataProvider(Of T), ByVal entity As T) As Task
#If DEBUG Then
			Await Task.Delay(500)
#End If
			Await Task.Run(Sub() dataProvider.Update(entity))
		End Function
		<System.Runtime.CompilerServices.Extension> _
		Public Async Function CreateAsync(Of T As Class)(ByVal dataProvider As IDataProvider(Of T), ByVal entity As T) As Task
#If DEBUG Then
			Await Task.Delay(500)
#End If
			Await Task.Run(Sub() dataProvider.Create(entity))
		End Function
	End Module
End Namespace
