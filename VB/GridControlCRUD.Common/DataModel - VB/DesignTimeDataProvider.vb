Imports DevExpress.Xpf.Data
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Linq.Expressions

Namespace DevExpress.CRUD.DataModel
	Public Class DesignTimeDataProvider(Of T As Class)
		Implements IDataProvider(Of T)

		Private ReadOnly createEntity As Func(Of Integer, T)
		Private ReadOnly count As Integer

		Public Sub New(ByVal createEntity As Func(Of Integer, T), Optional ByVal count As Integer = 5)
			Me.createEntity = createEntity
			Me.count = count
		End Sub

		Private Function IDataProviderGeneric_Read() As IList(Of T) Implements IDataProvider(Of T).Read
			Return Enumerable.Range(0, count).Select(createEntity).ToList()
		End Function

		Private Function IDataProviderGeneric_GetQueryableResult(Of TResult)(ByVal getResult As Func(Of IQueryable(Of T), TResult)) As TResult Implements IDataProvider(Of T).GetQueryableResult
			Dim queryable = DirectCast(Me, IDataProvider(Of T)).Read().AsQueryable()
			Return getResult(queryable)
		End Function

		Private Sub IDataProviderGeneric_Create(ByVal obj As T) Implements IDataProvider(Of T).Create
			Throw New NotSupportedException()
		End Sub
		Private Sub IDataProviderGeneric_Delete(ByVal obj As T) Implements IDataProvider(Of T).Delete
			Throw New NotSupportedException()
		End Sub
		Private Sub IDataProviderGeneric_Update(ByVal obj As T) Implements IDataProvider(Of T).Update
			Throw New NotSupportedException()
		End Sub

		Private ReadOnly Property IDataProviderGeneric_KeyProperty() As String Implements IDataProvider(Of T).KeyProperty
			Get
'INSTANT VB TODO TASK: Throw expressions are not converted by Instant VB:
'ORIGINAL LINE: return throw new NotSupportedException();
				Return throw New NotSupportedException()
			End Get
		End Property
	End Class
End Namespace
