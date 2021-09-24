Imports System.Linq
Class MainWindow
    Public Sub New()
        InitializeComponent()
        Refresh()
    End Sub
    Private _UnitOfWork As DevExpress.Xpo.UnitOfWork

    Private Sub Refresh()
        _UnitOfWork = New DevExpress.Xpo.UnitOfWork()
        Dim xpCollection = New DevExpress.Xpo.XPCollection(Of Issues.User)(_UnitOfWork)
        xpCollection.Sorting.Add(New DevExpress.Xpo.SortProperty(NameOf(Issues.User.Oid), DevExpress.Xpo.DB.SortingDirection.Ascending))
        grid.ItemsSource = xpCollection
    End Sub

    Private Sub OnValidateRow(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.GridRowValidationEventArgs)
        _UnitOfWork.CommitChanges()
    End Sub

    Private Sub OnValidateDeleteRows(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.GridDeleteRowsValidationEventArgs)
        Dim row = CType(e.Rows.Single(), Issues.User)
        _UnitOfWork.Delete(row)
        _UnitOfWork.CommitChanges()
    End Sub

    Private Sub OnRefresh(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.RefreshEventArgs)
        Refresh()
    End Sub

End Class
