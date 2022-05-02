Imports DevExpress.Mvvm
Imports EntityFrameworkIssues.Issues
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Data.Linq
Imports System.Data.Entity
Imports System.Linq
Imports System.Collections
Imports DevExpress.Mvvm.Xpf
Imports System

Public Class MainViewModel
    Inherits ViewModelBase
    Private _ItemsSource As EntityInstantFeedbackSource
    Public ReadOnly Property ItemsSource As EntityInstantFeedbackSource
        Get
            If _ItemsSource Is Nothing Then
                _ItemsSource = New EntityInstantFeedbackSource With {
                    .KeyExpression = NameOf(Issue.Id)
                }
                AddHandler _ItemsSource.GetQueryable, Sub(sender, e)
                                                          Dim context = New IssuesContext()
                                                          e.QueryableSource = context.Issues.AsNoTracking()
                                                      End Sub
            End If
            Return _ItemsSource
        End Get
    End Property
    Private _Users As IList
    Public ReadOnly Property Users As IList
        Get
            If _Users Is Nothing AndAlso Not DevExpress.Mvvm.ViewModelBase.IsInDesignMode Then
                Dim context = New IssuesContext()
                _Users = context.Users.[Select](Function(user) New With {
                    .Id = user.Id,
                    .Name = user.FirstName & " " + user.LastName
                }).ToArray()
            End If
            Return _Users
        End Get
    End Property
    <Command>
    Public Sub DataSourceRefresh(ByVal args As DataSourceRefreshArgs)
        _Users = Nothing
        RaisePropertyChanged(Nameof(Users))
    End Sub
    <Command>
    Public Sub CreateEditEntityViewModel(ByVal args As CreateEditItemViewModelArgs)
        Dim context = New IssuesContext()
        Dim item As Issue

        If args.IsNewItem Then
            item = New Issue() With {
                .Created = Date.Now
            }
            context.Entry(item).State = EntityState.Added
        Else
            item = context.Issues.Find(args.Key)
        End If

        args.ViewModel = New EditItemViewModel(item, New EditIssueInfo(context, Users), title:=If(args.IsNewItem, "New ", "Edit ") & NameOf(Issue))
    End Sub
    <Command>
    Public Sub ValidateRow(ByVal args As EditFormRowValidationArgs)
        Dim context = CType(args.EditOperationContext, EditIssueInfo).DbContext
        context.SaveChanges()
    End Sub
    <Command>
    Public Sub ValidateRowDeletion(ByVal args As EditFormValidateRowDeletionArgs)
        Dim key = CInt(args.Keys.[Single]())
        Dim item = New Issue() With {
            .Id = key
        }
        Dim context = New IssuesContext()
        context.Entry(item).State = EntityState.Deleted
        context.SaveChanges()
    End Sub

End Class