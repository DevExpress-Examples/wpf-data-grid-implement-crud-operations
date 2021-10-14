Imports System.Linq
Class MainWindow
    Public Sub New()
        InitializeComponent()
        LoadData()
    End Sub
    Private _UnitOfWork As DevExpress.Xpo.UnitOfWork

    Private Sub LoadData()
        _UnitOfWork = New DevExpress.Xpo.UnitOfWork()
        Dim xpCollection = New DevExpress.Xpo.XPCollection(Of Issues.User)(_UnitOfWork)
        xpCollection.Sorting.Add(New DevExpress.Xpo.SortProperty(NameOf(Issues.User.Oid), DevExpress.Xpo.DB.SortingDirection.Ascending))
        grid.ItemsSource = xpCollection
    End Sub

    Private Sub OnValidateRow(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.GridRowValidationEventArgs)
        _UnitOfWork.CommitChanges()
    End Sub

    Private Sub OnValidateRowDeletion(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.GridDeleteRowsValidationEventArgs)
        Dim row = CType(e.Rows.Single(), Issues.User)
        _UnitOfWork.Delete(row)
        _UnitOfWork.CommitChanges()
    End Sub

    Private Sub OnDataSourceRefresh(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.DataSourceRefreshEventArgs)
        LoadData()
    End Sub

End Class
