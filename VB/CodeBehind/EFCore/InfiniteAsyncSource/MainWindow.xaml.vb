Imports DevExpress.Xpf.Data
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Class MainWindow
    Public Sub New()
        InitializeComponent()
        Dim source = New InfiniteAsyncSource With {
            .ElementType = GetType(Issues.Issue),
            .KeyProperty = NameOf(Issues.Issue.Id)
        }
        AddHandler source.FetchRows, AddressOf OnFetchRows
        AddHandler source.GetTotalSummaries, AddressOf OnGetTotalSummaries
        grid.ItemsSource = source
        LoadLookupData()
    End Sub

    Private Sub OnFetchRows(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Data.FetchRowsAsyncEventArgs)
        e.Result = Task.Run(Of DevExpress.Xpf.Data.FetchRowsResult)(Function()
                                                                        Dim context = New Issues.IssuesContext()
                                                                        Dim queryable = context.Issues.AsNoTracking().SortBy(e.SortOrder, defaultUniqueSortPropertyName:=NameOf(Issues.Issue.Id)).Where(MakeFilterExpression(e.Filter))
                                                                        Return queryable.Skip(e.Skip).Take(If(e.Take, 100)).ToArray()
                                                                    End Function)
    End Sub

    Private Sub OnGetTotalSummaries(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Data.GetSummariesAsyncEventArgs)
        e.Result = Task.Run(Function()
                                Dim context = New Issues.IssuesContext()
                                Dim queryable = context.Issues.Where(MakeFilterExpression(CType(e.Filter, DevExpress.Data.Filtering.CriteriaOperator)))
                                Return queryable.GetSummaries(e.Summaries)
                            End Function)
    End Sub

    Private Function MakeFilterExpression(ByVal filter As DevExpress.Data.Filtering.CriteriaOperator) As System.Linq.Expressions.Expression(Of System.Func(Of EFCoreIssues.Issues.Issue, Boolean))
        Dim converter = New DevExpress.Xpf.Data.GridFilterCriteriaToExpressionConverter(Of EFCoreIssues.Issues.Issue)()
        Return converter.Convert(filter)
    End Function

    Private Sub OnValidateRow(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.GridRowValidationEventArgs)

        Dim row = CType(e.Row, Issues.Issue)
        Dim context = New Issues.IssuesContext()
        context.Entry(row).State = If(e.IsNewItem, EntityState.Added, EntityState.Modified)
        Try
            context.SaveChanges()
        Finally
            context.Entry(row).State = EntityState.Detached
        End Try

    End Sub

    Private Sub OnValidateDeleteRows(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.GridDeleteRowsValidationEventArgs)
        Dim row = CType(e.Rows.Single(), Issues.Issue)
        Dim context = New Issues.IssuesContext()
        context.Entry(row).State = EntityState.Deleted
        context.SaveChanges()
    End Sub

    Private Sub LoadLookupData()
        Dim context = New EFCoreIssues.Issues.IssuesContext()
        usersLookup.ItemsSource = context.Users.[Select](Function(user) New With {
            .Id = user.Id,
            .Name = user.FirstName & " " + user.LastName
        }).ToArray()
    End Sub

    Private Sub OnRefresh(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.RefreshEventArgs)
        LoadLookupData()
    End Sub

End Class
