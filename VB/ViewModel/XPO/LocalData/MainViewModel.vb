Imports DevExpress.Mvvm
Imports XPOIssues.Issues
Imports DevExpress.Xpo
Imports DevExpress.Mvvm.DataAnnotations
Imports System.Linq
Imports System.Collections.Generic
Imports DevExpress.Mvvm.Xpf

Public Class MainViewModel
    Inherits ViewModelBase
    Private _UnitOfWork As UnitOfWork
    Private _ItemsSource As IList(Of User)
    Public ReadOnly Property ItemsSource As IList(Of User)
        Get
            If _ItemsSource Is Nothing AndAlso Not DevExpress.Mvvm.ViewModelBase.IsInDesignMode Then
                _UnitOfWork = New UnitOfWork()
                Dim xpCollection = New XPCollection(Of User)(_UnitOfWork)
                xpCollection.Sorting.Add(New SortProperty(NameOf(User.Oid), DevExpress.Xpo.DB.SortingDirection.Ascending))
                _ItemsSource = xpCollection
            End If
            Return _ItemsSource
        End Get
    End Property
    <Command>
    Public Sub ValidateRow(ByVal args As RowValidationArgs)
        _UnitOfWork.CommitChanges()
    End Sub
    <Command>
    Public Sub ValidateRowDeletion(ByVal args As ValidateRowDeletionArgs)
        Dim item = CType(args.Items.Single(), User)
        _UnitOfWork.Delete(item)
        _UnitOfWork.CommitChanges()
    End Sub
    <Command>
    Public Sub DataSourceRefresh(ByVal args As DataSourceRefreshArgs)
        _ItemsSource = Nothing
        _UnitOfWork = Nothing
        RaisePropertyChanged(Nameof(ItemsSource))
    End Sub

End Class