Imports System
Imports System.Collections.Generic
Imports System.Data.Entity
Imports System.Linq
Imports System.Linq.Expressions

Namespace DevExpress.CRUD.DataModel.EntityFramework

    Public Class EntityFrameworkDataProvider(Of TContext As DbContext, TEntity As Class, T As Class)
        Implements IDataProvider(Of T)

        Protected ReadOnly createContext As Func(Of TContext)

        Protected ReadOnly getDbSet As Func(Of TContext, DbSet(Of TEntity))

        Private ReadOnly getEntityExpression As Expression(Of Func(Of TEntity, T))

        Public Sub New(ByVal createContext As Func(Of TContext), ByVal getDbSet As Func(Of TContext, DbSet(Of TEntity)), ByVal getEnityExpression As Expression(Of Func(Of TEntity, T)))
            Me.createContext = createContext
            Me.getDbSet = getDbSet
            getEntityExpression = getEnityExpression
        End Sub

        Private Function Read() As IList(Of T) Implements IDataProvider(Of T).Read
            Using context = createContext()
                Dim query = getDbSet(context).[Select](getEntityExpression)
                Return query.ToList()
            End Using
        End Function
    End Class
End Namespace
