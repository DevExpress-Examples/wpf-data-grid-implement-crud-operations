Imports DevExpress.Mvvm
Imports System.Collections.Generic
Imports XPOIssues.Issues

Public Class EditIssueInfo
    Inherits BindableBase

    Public Sub New(ByVal unitOfWork As DevExpress.Xpo.UnitOfWork, ByVal users As IList)
        Me.UnitOfWork = unitOfWork
        Me.Users = users
    End Sub

    Public ReadOnly Property UnitOfWork As DevExpress.Xpo.UnitOfWork
    Public ReadOnly Property Users As IList
End Class
