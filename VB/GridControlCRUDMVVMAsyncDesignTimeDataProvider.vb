Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace DevExpress.CRUD.DataModel

    Public Class DesignTimeDataProvider(Of T As Class)
        Implements ICRUDDataProvider(Of T)

        Private ReadOnly createEntity As Func(Of Integer, T)

        Private ReadOnly count As Integer

        Public Sub New(ByVal createEntity As Func(Of Integer, T), ByVal Optional count As Integer = 5)
            Me.createEntity = createEntity
            Me.count = count
        End Sub

        Private Function Read() As IList(Of T) Implements IDataProvider(Of T).Read
            Return Enumerable.Range(0, count).[Select](createEntity).ToList()
        End Function

        Private Sub Create(ByVal obj As T) Implements ICRUDDataProvider(Of T).Create
            Throw New NotSupportedException()
        End Sub

        Private Sub Delete(ByVal obj As T) Implements ICRUDDataProvider(Of T).Delete
            Throw New NotSupportedException()
        End Sub

        Private Sub Update(ByVal obj As T) Implements ICRUDDataProvider(Of T).Update
            Throw New NotSupportedException()
        End Sub
    End Class
End Namespace
