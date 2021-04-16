Imports DevExpress.Xpf.Data
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Linq.Expressions

Namespace DevExpress.CRUD.DataModel
	Public Interface IDataProvider(Of T As Class)
		Function Read() As IList(Of T)
		Sub Create(ByVal obj As T)
		Sub Update(ByVal obj As T)
		Sub Delete(ByVal obj As T)
		Function GetQueryableResult(Of TResult)(ByVal getResult As Func(Of IQueryable(Of T), TResult)) As TResult 'used for virtual sources
		ReadOnly Property KeyProperty() As String
	End Interface
End Namespace
