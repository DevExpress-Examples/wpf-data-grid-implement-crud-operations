Imports DevExpress.Mvvm
Imports System.Linq

Public Class MainViewModel
    Inherits ViewModelBase
    Private _UnitOfWork As DevExpress.Xpo.UnitOfWork
    Private _ItemsSource As System.Collections.Generic.IList(Of XPOIssues.Issues.User)

    Public ReadOnly Property ItemsSource As System.Collections.Generic.IList(Of XPOIssues.Issues.User)
        Get
            If _ItemsSource Is Nothing AndAlso Not IsInDesignMode Then
                _UnitOfWork = New DevExpress.Xpo.UnitOfWork()
                Dim xpCollection = New DevExpress.Xpo.XPCollection(Of Issues.User)(_UnitOfWork)
                xpCollection.Sorting.Add(New DevExpress.Xpo.SortProperty(NameOf(Issues.User.Oid), DevExpress.Xpo.DB.SortingDirection.Ascending))
                _ItemsSource = xpCollection
            End If
            Return _ItemsSource
        End Get
    End Property
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub ValidateRow(ByVal args As DevExpress.Mvvm.Xpf.RowValidationArgs)
        _UnitOfWork.CommitChanges()
    End Sub
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub ValidateDeleteRows(ByVal args As DevExpress.Mvvm.Xpf.DeleteRowsValidationArgs)
        Dim item = CType(args.Items.Single(), Issues.User)
        _UnitOfWork.Delete(item)
        _UnitOfWork.CommitChanges()
    End Sub
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub RefreshDataSource(ByVal args As DevExpress.Mvvm.Xpf.RefreshDataSourceArgs)
        _ItemsSource = Nothing
        _UnitOfWork = Nothing
        RaisePropertyChanged(Nameof(ItemsSource))
    End Sub

End Class