Imports DevExpress.Xpf.Data
Imports System
Imports System.Collections.Generic
Imports System.Data.Entity
Imports System.Data.Entity.Validation
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Text

Namespace DevExpress.CRUD.DataModel.EntityFramework
	Public Class EntityFrameworkDataProvider(Of TContext As DbContext, TEntity As Class, T As Class)
		Implements IDataProvider(Of T)

		Protected ReadOnly createContext As Func(Of TContext)
		Protected ReadOnly getDbSet As Func(Of TContext, DbSet(Of TEntity))
		Protected ReadOnly getEntityExpression As Expression(Of Func(Of TEntity, T))
'INSTANT VB NOTE: The field keyProperty was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly keyProperty_Conflict As String

		Private ReadOnly getKey As Func(Of T, Object)
		Private ReadOnly getEntityKey As Func(Of TEntity, Object)
		Private ReadOnly setKey As Action(Of T, Object)
		Private ReadOnly applyProperties As Action(Of T, TEntity)

		Public Sub New(ByVal createContext As Func(Of TContext), ByVal getDbSet As Func(Of TContext, DbSet(Of TEntity)), ByVal getEnityExpression As Expression(Of Func(Of TEntity, T)), Optional ByVal keyProperty As String = Nothing, Optional ByVal getKey As Func(Of T, Object) = Nothing, Optional ByVal getEntityKey As Func(Of TEntity, Object) = Nothing, Optional ByVal setKey As Action(Of T, Object) = Nothing, Optional ByVal applyProperties As Action(Of T, TEntity) = Nothing)
			Me.createContext = createContext
			Me.getDbSet = getDbSet
			Me.getEntityExpression = getEnityExpression

			Me.keyProperty_Conflict = keyProperty
'INSTANT VB TODO TASK: Throw expressions are not converted by Instant VB:
'ORIGINAL LINE: this.getKey = getKey ?? (underscore => throw new NotSupportedException());
'INSTANT VB TODO TASK: Underscore 'discards' are not converted by Instant VB:
			Me.getKey = If(getKey, (Function(underscore) throw New NotSupportedException()))
'INSTANT VB TODO TASK: Throw expressions are not converted by Instant VB:
'ORIGINAL LINE: this.getEntityKey = getEntityKey ?? (underscore => throw new NotSupportedException());
'INSTANT VB TODO TASK: Underscore 'discards' are not converted by Instant VB:
			Me.getEntityKey = If(getEntityKey, (Function(underscore) throw New NotSupportedException()))
'INSTANT VB TODO TASK: Throw expressions are not converted by Instant VB:
'ORIGINAL LINE: this.setKey = setKey ?? ((underscore, __) => throw new NotSupportedException());
'INSTANT VB TODO TASK: Underscore 'discards' are not converted by Instant VB:
			Me.setKey = If(setKey, (Function(underscore, __) throw New NotSupportedException()))
'INSTANT VB TODO TASK: Throw expressions are not converted by Instant VB:
'ORIGINAL LINE: this.applyProperties = applyProperties ?? ((underscore, __) => throw new NotSupportedException());
'INSTANT VB TODO TASK: Underscore 'discards' are not converted by Instant VB:
			Me.applyProperties = If(applyProperties, (Function(underscore, __) throw New NotSupportedException()))
		End Sub

		Private Function IDataProviderGeneric_Read() As IList(Of T) Implements IDataProvider(Of T).Read
			Using context = createContext()
				Dim query = getDbSet(context).Select(getEntityExpression)
				Return query.ToList()
			End Using
		End Function

		Private Sub IDataProviderGeneric_Delete(ByVal obj As T) Implements IDataProvider(Of T).Delete
			Using context = createContext()
				Dim entity = getDbSet(context).Find(getKey(obj))
				If entity Is Nothing Then
					Throw New NotImplementedException("The modified row no longer exists in the database. Handle this case according to your requirements.")
				End If
				getDbSet(context).Remove(entity)
				SaveChanges(context)
			End Using
		End Sub

		Private Sub IDataProviderGeneric_Create(ByVal obj As T) Implements IDataProvider(Of T).Create
			Using context = createContext()
				Dim entity = getDbSet(context).Create()
				getDbSet(context).Add(entity)
				applyProperties(obj, entity)
				SaveChanges(context)
				setKey(obj, getEntityKey(entity))
			End Using
		End Sub

		Private Sub IDataProviderGeneric_Update(ByVal obj As T) Implements IDataProvider(Of T).Update
			Using context = createContext()
				Dim entity = getDbSet(context).Find(getKey(obj))
				If entity Is Nothing Then
					Throw New NotImplementedException("The modified row does not exist in a database anymore. Handle this case according to your requirements.")
				End If
				applyProperties(obj, entity)
				SaveChanges(context)
			End Using
		End Sub

		Private Function IDataProviderGeneric_GetQueryableResult(Of TResult)(ByVal getResult As Func(Of IQueryable(Of T), TResult)) As TResult Implements IDataProvider(Of T).GetQueryableResult
			Using context = createContext()
				Dim queryable = getDbSet(context).Select(getEntityExpression)
				Return getResult(queryable)
			End Using
		End Function

		Private ReadOnly Property IDataProviderGeneric_KeyProperty() As String Implements IDataProvider(Of T).KeyProperty
			Get
				Return keyProperty_Conflict
			End Get
		End Property

		Private Shared Sub SaveChanges(ByVal context As TContext)
			Try
				context.SaveChanges()
			Catch e As Exception
				Throw ConvertException(e)
			End Try
		End Sub

		Private Shared Function ConvertException(ByVal e As Exception) As DbException
			Dim entityValidationException = TryCast(e, DbEntityValidationException)
			If entityValidationException IsNot Nothing Then
				Dim stringBuilder As New StringBuilder()
				For Each validationResult In entityValidationException.EntityValidationErrors
					For Each [error] In validationResult.ValidationErrors
						If stringBuilder.Length > 0 Then
							stringBuilder.AppendLine()
						End If
						stringBuilder.Append([error].PropertyName & ": " & [error].ErrorMessage)
					Next [error]
				Next validationResult
				Return New DbException(stringBuilder.ToString(), entityValidationException)
			End If
			Return New DbException("An error has occurred while updating the database.", entityValidationException)
		End Function
	End Class
	Public Class DbException
		Inherits Exception

		Public Sub New(ByVal message As String, ByVal innerException As Exception)
			MyBase.New(message, innerException)
		End Sub
	End Class
End Namespace
