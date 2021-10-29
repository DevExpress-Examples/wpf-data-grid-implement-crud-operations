Imports DevExpress.Mvvm
Imports System.Collections.Generic
Imports EntityFrameworkIssues.Issues

Public Class EditIssueInfo
    Inherits BindableBase

    Public Sub New(ByVal dbContext As IssuesContext, ByVal users As IList)
        Me.DbContext = dbContext
        Me.Users = users
    End Sub

    Public ReadOnly Property DbContext As IssuesContext
    Public ReadOnly Property Users As IList
End Class
