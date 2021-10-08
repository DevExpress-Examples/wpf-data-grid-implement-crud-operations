Imports DevExpress.Xpf.Data
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Data.Entity
Imports EntityFrameworkIssues.Issues
Imports DevExpress.Mvvm.Xpf
Imports System
Imports System.Collections
Class MainWindow
    Public Sub New()
        InitializeComponent()
        Dim source = New DevExpress.Data.Linq.EntityInstantFeedbackSource With {
            .KeyExpression = NameOf(Issues.Issue.Id)
        }
        AddHandler source.GetQueryable, Sub(sender, e)
                                            Dim context = New Issues.IssuesContext()
                                            e.QueryableSource = context.Issues.AsNoTracking()
                                        End Sub
        grid.ItemsSource = source
        LoadLookupData()
    End Sub

    Private Sub LoadLookupData()
        Dim context = New EntityFrameworkIssues.Issues.IssuesContext()
        usersLookup.ItemsSource = context.Users.[Select](Function(user) New With {
            .Id = user.Id,
            .Name = user.FirstName & " " + user.LastName
        }).ToArray()
    End Sub

    Private Sub OnDataSourceRefresh(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.DataSourceRefreshEventArgs)
        LoadLookupData()
    End Sub

    Private Sub OnCreateEditEntityViewModel(ByVal sender As System.Object, ByVal e As DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs)
        Dim context = New IssuesContext()
        Dim item As Issue

        If e.Key IsNot Nothing Then
            item = context.Issues.Find(e.Key)
        Else
            item = New Issue() With {
                .Created = Date.Now
            }
            context.Entry(item).State = EntityState.Added
        End If

        e.ViewModel = New EditItemViewModel(item, New EditIssueInfo(context, CType(usersLookup.ItemsSource, IList)))
    End Sub

    Private Sub OnValidateRow(ByVal sender As System.Object, ByVal e As DevExpress.Mvvm.Xpf.EditFormRowValidationArgs)
        Dim context = CType(e.Tag, EditIssueInfo).Context
        context.SaveChanges()
    End Sub

    Private Sub OnValidateRowDeletion(ByVal sender As System.Object, ByVal e As DevExpress.Mvvm.Xpf.EditFormDeleteRowsValidationArgs)
        Dim key = CInt(e.Keys.[Single]())
        Dim item = New Issue() With {
            .Id = key
        }
        Dim context = New IssuesContext()
        context.Entry(item).State = EntityState.Deleted
        context.SaveChanges()
    End Sub

End Class
