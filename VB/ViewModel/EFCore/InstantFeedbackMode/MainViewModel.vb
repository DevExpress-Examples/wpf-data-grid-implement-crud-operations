Imports DevExpress.Mvvm
Imports DevExpress.Xpf.Data
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports EFCoreIssues.Issues
Imports DevExpress.Mvvm.Xpf
Imports System

Public Class MainViewModel
    Inherits ViewModelBase
    Private _InstantFeedbackSource As DevExpress.Data.Linq.EntityInstantFeedbackSource

    Public ReadOnly Property InstantFeedbackSource As DevExpress.Data.Linq.EntityInstantFeedbackSource
        Get
            If _InstantFeedbackSource Is Nothing Then
                _InstantFeedbackSource = New DevExpress.Data.Linq.EntityInstantFeedbackSource With {
                    .KeyExpression = NameOf(Issues.Issue.Id)
                }
                AddHandler _InstantFeedbackSource.GetQueryable, Sub(sender, e)
                                                                    Dim context = New Issues.IssuesContext()
                                                                    e.QueryableSource = context.Issues.AsNoTracking()
                                                                End Sub
            End If
            Return _InstantFeedbackSource
        End Get
    End Property
    Private _Users As System.Collections.IList

    Public ReadOnly Property Users As System.Collections.IList
        Get
            If _Users Is Nothing AndAlso Not IsInDesignMode Then
                Dim context = New EFCoreIssues.Issues.IssuesContext()
                _Users = context.Users.[Select](Function(user) New With {
                    .Id = user.Id,
                    .Name = user.FirstName & " " + user.LastName
                }).ToArray()
            End If
            Return _Users
        End Get
    End Property
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub RefreshDataSource(ByVal args As DevExpress.Mvvm.Xpf.RefreshDataSourceArgs)
        _Users = Nothing
        RaisePropertyChanged(Nameof(Users))
    End Sub
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub CreateEditEntityViewModel(ByVal args As DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs)
        Dim context = New IssuesContext()
        Dim item As Issue

        If args.Key IsNot Nothing Then
            item = context.Issues.Find(args.Key)
        Else
            item = New Issue() With {
                .Created = Date.Now
            }
            context.Entry(item).State = EntityState.Added
        End If

        args.ViewModel = New EditItemViewModel(item, New EditIssueInfo(context, Users))
    End Sub
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub ValidateRow(ByVal args As DevExpress.Mvvm.Xpf.EditFormRowValidationArgs)
        Dim context = CType(args.Tag, EditIssueInfo).Context
        context.SaveChanges()
    End Sub
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub ValidateDeleteRows(ByVal args As DevExpress.Mvvm.Xpf.EditFormDeleteRowsValidationArgs)
        Dim key = CInt(args.Keys.[Single]())
        Dim item = New Issue() With {
            .Id = key
        }
        Dim context = New IssuesContext()
        context.Entry(item).State = EntityState.Deleted
        context.SaveChanges()
    End Sub

End Class