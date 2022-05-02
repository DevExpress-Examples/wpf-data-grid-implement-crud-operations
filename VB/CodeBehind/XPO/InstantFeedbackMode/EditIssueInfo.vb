Imports DevExpress.Xpo
Imports DevExpress.Mvvm
Imports System.Collections.Generic
Imports DevExpress.Xpf.Grid
Imports XPOIssues.Issues

Public Class EditIssueInfo
    Inherits BindableBase

    Public Sub New(ByVal unitOfWork As UnitOfWork, ByVal users As IList)
        Me.UnitOfWork = unitOfWork
        Me.Users = users
    End Sub

    Public ReadOnly Property UnitOfWork As UnitOfWork
    Public ReadOnly Property Users As IList
End Class
