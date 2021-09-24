Imports DevExpress.Mvvm
Imports System.Collections.Generic
Imports EFCoreIssues.Issues

Public Class EditIssueInfo
    Inherits BindableBase

    Public Sub New(ByVal context As IssuesContext, ByVal users As IList)
        Me.Context = context
        Me.Users = users
    End Sub

    Public ReadOnly Property Context As IssuesContext
    Public ReadOnly Property Users As IList
End Class
