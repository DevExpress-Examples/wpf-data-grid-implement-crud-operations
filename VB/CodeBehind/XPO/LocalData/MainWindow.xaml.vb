Imports XPOIssues.Issues
Imports DevExpress.Xpo
Imports System.Linq
Imports DevExpress.Xpf.Grid
Class MainWindow
    Public Sub New()
        InitializeComponent()
        LoadData()
    End Sub
    Private _UnitOfWork As UnitOfWork

    Private Sub LoadData()
        _UnitOfWork = New UnitOfWork()
        Dim xpCollection = New XPCollection(Of User)(_UnitOfWork)
        xpCollection.Sorting.Add(New SortProperty(NameOf(User.Oid), DevExpress.Xpo.DB.SortingDirection.Ascending))
        grid.ItemsSource = xpCollection
    End Sub

    Private Sub OnValidateRow(ByVal sender As Object, ByVal e As GridRowValidationEventArgs)
        _UnitOfWork.CommitChanges()
    End Sub

    Private Sub OnValidateRowDeletion(ByVal sender As Object, ByVal e As GridValidateRowDeletionEventArgs)
        Dim row = CType(e.Rows.Single(), User)
        _UnitOfWork.Delete(row)
        _UnitOfWork.CommitChanges()
    End Sub

    Private Sub OnDataSourceRefresh(ByVal sender As Object, ByVal e As DataSourceRefreshEventArgs)
        LoadData()
    End Sub

End Class
