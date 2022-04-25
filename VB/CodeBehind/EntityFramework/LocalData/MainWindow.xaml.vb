Imports EntityFrameworkIssues.Issues
Imports System.Data.Entity
Imports System.Linq
Imports DevExpress.Xpf.Grid
Class MainWindow
    Public Sub New()
        InitializeComponent()
        LoadData()
    End Sub
    Private _Context As IssuesContext

    Private Sub LoadData()
        _Context = New IssuesContext()
        grid.ItemsSource = _Context.Users.ToList()
    End Sub

    Private Sub OnValidateRow(ByVal sender As Object, ByVal e As GridRowValidationEventArgs)
        Dim row = CType(e.Row, User)
        If (e.IsNewItem) Then _Context.Users.Add(row)
        _Context.SaveChanges()
    End Sub

    Private Sub OnValidateRowDeletion(ByVal sender As Object, ByVal e As GridValidateRowDeletionEventArgs)
        Dim row = CType(e.Rows.Single(), User)
        _Context.Users.Remove(row)
        _Context.SaveChanges()
    End Sub

    Private Sub OnDataSourceRefresh(ByVal sender As Object, ByVal e As DataSourceRefreshEventArgs)
        LoadData()
    End Sub

End Class
