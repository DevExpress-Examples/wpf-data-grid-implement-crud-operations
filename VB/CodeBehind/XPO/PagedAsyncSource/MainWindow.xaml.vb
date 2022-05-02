Imports XPOIssues.Issues
Imports System
Imports System.Linq.Expressions
Imports DevExpress.Data.Filtering
Imports DevExpress.Xpf.Data
Imports System.Linq
Imports System.Threading.Tasks
Imports DevExpress.Xpo
Imports DevExpress.Xpf.Grid
Class MainWindow
    Public Sub New()
        InitializeComponent()
        Using session = New Session()
            Dim classInfo = session.GetClassInfo(Of Issue)()
            Dim properties = classInfo.Members.Where(Function(member) member.IsPublic AndAlso member.IsPersistent).[Select](Function(member) member.Name).ToArray()
            _DetachedObjectsHelper = DetachedObjectsHelper(Of Issue).Create(classInfo.KeyProperty.Name, properties)
        End Using
        Dim source = New PagedAsyncSource With {
            .CustomProperties = _DetachedObjectsHelper.Properties,
            .KeyProperty = nameof(Issue.Oid),
            .PageNavigationMode = PageNavigationMode.ArbitraryWithTotalPageCount
        }
        AddHandler source.FetchPage, AddressOf OnFetchPage
        AddHandler source.GetTotalSummaries, AddressOf OnGetTotalSummaries
        grid.ItemsSource = source
        LoadLookupData()
    End Sub

    Private Function MakeFilterExpression(ByVal filter As CriteriaOperator) As Expression(Of Func(Of Issue, Boolean))
        Dim converter = New GridFilterCriteriaToExpressionConverter(Of Issue)()
        Return converter.Convert(filter)
    End Function
    Private _DetachedObjectsHelper As DetachedObjectsHelper(Of Issue)

    Private Sub OnFetchPage(ByVal sender As Object, ByVal e As FetchPageAsyncEventArgs)
        e.Result = Task.Run(Of FetchRowsResult)(Function()
                                                    Const pageTakeCount As Integer = 5
                                                    Using session = New Session()
                                                        Dim queryable = session.Query(Of Issue)().SortBy(e.SortOrder, defaultUniqueSortPropertyName:=NameOf(Issue.Oid)).Where(MakeFilterExpression(CType(e.Filter, CriteriaOperator)))
                                                        Dim items = queryable.Skip(e.Skip).Take(e.Take * pageTakeCount).ToArray()
                                                        Return _DetachedObjectsHelper.ConvertToDetachedObjects(items)
                                                    End Using
                                                End Function)
    End Sub

    Private Sub OnGetTotalSummaries(ByVal sender As Object, ByVal e As GetSummariesAsyncEventArgs)
        e.Result = Task.Run(Function()
                                Using session = New Session()
                                    Return session.Query(Of Issue)().Where(MakeFilterExpression(e.Filter)).GetSummaries(e.Summaries)
                                End Using
                            End Function)
    End Sub

    Private Sub OnValidateRow(ByVal sender As Object, ByVal e As GridRowValidationEventArgs)
        Using unitOfWork = New UnitOfWork()
            Dim item = If(e.IsNewItem, New Issue(unitOfWork), unitOfWork.GetObjectByKey(Of Issue)(_DetachedObjectsHelper.GetKey(e.Row)))
            _DetachedObjectsHelper.ApplyProperties(item, e.Row)
            unitOfWork.CommitChanges()

            If e.IsNewItem Then
                _DetachedObjectsHelper.SetKey(e.Row, item.Oid)
            End If
        End Using
    End Sub

    Private Sub LoadLookupData()
        Dim session = New Session()
        usersLookup.ItemsSource = session.Query(Of User).OrderBy(Function(user) user.Oid).[Select](Function(user) New With {
            .Id = user.Oid,
            .Name = user.FirstName & " " + user.LastName
        }).ToArray()
    End Sub

    Private Sub OnDataSourceRefresh(ByVal sender As Object, ByVal e As DataSourceRefreshEventArgs)
        LoadLookupData()
    End Sub

End Class
