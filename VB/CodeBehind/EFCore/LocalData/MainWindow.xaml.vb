Imports System.Linq
Class MainWindow
    Public Sub New()
        InitializeComponent()
        LoadData()
    End Sub
    Private _Context As Issues.IssuesContext

    Private Sub LoadData()
        _Context = New Issues.IssuesContext()
        grid.ItemsSource = _Context.Users.ToList()
    End Sub

    Private Sub OnValidateRow(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.GridRowValidationEventArgs)
        Dim row = CType(e.Row, Issues.User)
        If (e.IsNewItem) Then _Context.Users.Add(row)
        _Context.SaveChanges()
    End Sub

    Private Sub OnValidateRowDeletion(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.GridDeleteRowsValidationEventArgs)
        Dim row = CType(e.Rows.Single(), Issues.User)
        _Context.Users.Remove(row)
        _Context.SaveChanges()
    End Sub

    Private Sub OnDataSourceRefresh(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.DataSourceRefreshEventArgs)
        LoadData()
    End Sub

End Class
