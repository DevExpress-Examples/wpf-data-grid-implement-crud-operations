﻿Imports EFCoreIssues.Issues
Imports System
Imports System.Linq.Expressions
Imports DevExpress.Data.Filtering
Imports DevExpress.Xpf.Data
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports DevExpress.Xpf.Grid
Class MainWindow
    Public Sub New()
        InitializeComponent()
        Dim source = New InfiniteAsyncSource With {
            .ElementType = GetType(Issue),
            .KeyProperty = NameOf(Issue.Id)
        }
        AddHandler source.FetchRows, AddressOf OnFetchRows
        AddHandler source.GetTotalSummaries, AddressOf OnGetTotalSummaries
        grid.ItemsSource = source
        LoadLookupData()
    End Sub

    Private Function MakeFilterExpression(ByVal filter As CriteriaOperator) As Expression(Of Func(Of Issue, Boolean))
        Dim converter = New GridFilterCriteriaToExpressionConverter(Of Issue)()
        Return converter.Convert(filter)
    End Function

    Private Sub OnFetchRows(ByVal sender As Object, ByVal e As FetchRowsAsyncEventArgs)
        e.Result = Task.Run(Of FetchRowsResult)(Function()
                                                    Dim context = New IssuesContext()
                                                    Dim queryable = context.Issues.AsNoTracking().SortBy(e.SortOrder, defaultUniqueSortPropertyName:=NameOf(Issue.Id)).Where(MakeFilterExpression(e.Filter))
                                                    Return queryable.Skip(e.Skip).Take(If(e.Take, 100)).ToArray()
                                                End Function)
    End Sub

    Private Sub OnGetTotalSummaries(ByVal sender As Object, ByVal e As GetSummariesAsyncEventArgs)
        e.Result = Task.Run(Function()
                                Dim context = New IssuesContext()
                                Dim queryable = context.Issues.Where(MakeFilterExpression(CType(e.Filter, CriteriaOperator)))
                                Return queryable.GetSummaries(e.Summaries)
                            End Function)
    End Sub

    Private Sub OnValidateRow(ByVal sender As Object, ByVal e As GridRowValidationEventArgs)

        Dim row = CType(e.Row, Issue)
        Dim context = New IssuesContext()
        context.Entry(row).State = If(e.IsNewItem, EntityState.Added, EntityState.Modified)
        Try
            context.SaveChanges()
        Finally
            context.Entry(row).State = EntityState.Detached
        End Try

    End Sub

    Private Sub OnValidateRowDeletion(ByVal sender As Object, ByVal e As GridValidateRowDeletionEventArgs)
        Dim row = CType(e.Rows.Single(), Issue)
        Dim context = New IssuesContext()
        context.Entry(row).State = EntityState.Deleted
        context.SaveChanges()
    End Sub

    Private Sub LoadLookupData()
        Dim context = New IssuesContext()
        usersLookup.ItemsSource = context.Users.[Select](Function(user) New With {
            .Id = user.Id,
            .Name = user.FirstName & " " + user.LastName
        }).ToArray()
    End Sub

    Private Sub OnDataSourceRefresh(ByVal sender As Object, ByVal e As DataSourceRefreshEventArgs)
        LoadLookupData()
    End Sub

End Class
