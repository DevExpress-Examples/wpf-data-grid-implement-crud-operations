Imports DevExpress.Mvvm
Imports System.Linq

Public Class MainViewModel
    Inherits ViewModelBase
    Private _Context As Issues.IssuesContext
    Private _ItemsSource As System.Collections.Generic.IList(Of EntityFrameworkIssues.Issues.User)

    Public ReadOnly Property ItemsSource As System.Collections.Generic.IList(Of EntityFrameworkIssues.Issues.User)
        Get
            If _ItemsSource Is Nothing AndAlso Not IsInDesignMode Then
                _Context = New Issues.IssuesContext()
                _ItemsSource = _Context.Users.ToList()
            End If
            Return _ItemsSource
        End Get
    End Property
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub ValidateRow(ByVal args As DevExpress.Mvvm.Xpf.RowValidationArgs)
        Dim item = CType(args.Item, Issues.User)
        If args.IsNewItem Then _Context.Users.Add(item)
        _Context.SaveChanges()
    End Sub
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub ValidateRowDeletion(ByVal args As DevExpress.Mvvm.Xpf.ValidateRowDeletionArgs)
        Dim item = CType(args.Items.Single(), Issues.User)
        _Context.Users.Remove(item)
        _Context.SaveChanges()
    End Sub
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub DataSourceRefresh(ByVal args As DevExpress.Mvvm.Xpf.DataSourceRefreshArgs)
        _ItemsSource = Nothing
        _Context = Nothing
        RaisePropertyChanged(Nameof(ItemsSource))
    End Sub

End Class