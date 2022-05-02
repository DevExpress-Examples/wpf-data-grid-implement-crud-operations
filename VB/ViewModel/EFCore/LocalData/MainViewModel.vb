Imports DevExpress.Mvvm
Imports EFCoreIssues.Issues
Imports DevExpress.Mvvm.DataAnnotations
Imports System.Linq
Imports System.Collections.Generic
Imports DevExpress.Mvvm.Xpf

Public Class MainViewModel
    Inherits ViewModelBase
    Private _Context As IssuesContext
    Private _ItemsSource As IList(Of User)
    Public ReadOnly Property ItemsSource As IList(Of User)
        Get
            If _ItemsSource Is Nothing AndAlso Not DevExpress.Mvvm.ViewModelBase.IsInDesignMode Then
                _Context = New IssuesContext()
                _ItemsSource = _Context.Users.ToList()
            End If
            Return _ItemsSource
        End Get
    End Property
    <Command>
    Public Sub ValidateRow(ByVal args As RowValidationArgs)
        Dim item = CType(args.Item, User)
        If args.IsNewItem Then _Context.Users.Add(item)
        _Context.SaveChanges()
    End Sub
    <Command>
    Public Sub ValidateRowDeletion(ByVal args As ValidateRowDeletionArgs)
        Dim item = CType(args.Items.Single(), User)
        _Context.Users.Remove(item)
        _Context.SaveChanges()
    End Sub
    <Command>
    Public Sub DataSourceRefresh(ByVal args As DataSourceRefreshArgs)
        _ItemsSource = Nothing
        _Context = Nothing
        RaisePropertyChanged(Nameof(ItemsSource))
    End Sub

End Class