Imports EFCoreIssues.Issues
Imports DevExpress.Data.Linq
Imports Microsoft.EntityFrameworkCore
Imports System.Linq
Imports DevExpress.Xpf.Grid
Imports DevExpress.Mvvm.Xpf
Imports System
Imports System.Collections
Class MainWindow
    Public Sub New()
        InitializeComponent()
        Dim context = New IssuesContext()
        Dim source = New EntityServerModeSource With {
            .KeyExpression = NameOf(Issue.Id),
            .QueryableSource = context.Issues.AsNoTracking()
        }
        grid.ItemsSource = source
        LoadLookupData()
    End Sub

    Private Sub LoadLookupData()
        Dim context = New IssuesContext()
        usersLookup.ItemsSource = context.Users.[Select](Function(user) New With {
            .Id = user.Id,
            .Name = user.FirstName & " " + user.LastName
        }).ToArray()
    End Sub

    Private Sub OnDataSourceRefresh(ByVal sender As Object, ByVal e As DataSourceRefreshEventArgs)
        LoadLookupData()
    End Sub

    Private Sub OnCreateEditEntityViewModel(ByVal sender As Object, ByVal e As CreateEditItemViewModelArgs)
        Dim context = New IssuesContext()
        Dim item As Issue

        If e.IsNewItem Then
            item = New Issue() With {
                .Created = Date.Now
            }
            context.Entry(item).State = EntityState.Added
        Else
            item = context.Issues.Find(e.Key)
        End If

        e.ViewModel = New EditItemViewModel(item, New EditIssueInfo(context, CType(usersLookup.ItemsSource, IList)), title:=If(e.IsNewItem, "New ", "Edit ") & NameOf(Issue))
    End Sub

    Private Sub OnValidateRow(ByVal sender As Object, ByVal e As EditFormRowValidationArgs)
        Dim context = CType(e.EditOperationContext, EditIssueInfo).DbContext
        context.SaveChanges()
    End Sub

    Private Sub OnValidateRowDeletion(ByVal sender As Object, ByVal e As EditFormValidateRowDeletionArgs)
        Dim key = CInt(e.Keys.[Single]())
        Dim item = New Issue() With {
            .Id = key
        }
        Dim context = New IssuesContext()
        context.Entry(item).State = EntityState.Deleted
        context.SaveChanges()
    End Sub

End Class
