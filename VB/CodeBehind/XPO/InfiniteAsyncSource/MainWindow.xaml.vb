Imports XPOIssues.Issues
Imports DevExpress.Xpo
Imports DevExpress.Xpf.Data
Imports System.Linq
Imports System.Threading.Tasks
Imports DevExpress.Xpf.Grid
Class MainWindow
    Public Sub New()
        InitializeComponent()
        Using session = New Session()
            Dim classInfo = session.GetClassInfo(Of Issue)()
            Dim properties = classInfo.Members.Where(Function(member) member.IsPublic AndAlso member.IsPersistent).[Select](Function(member) member.Name).ToArray()
            _DetachedObjectsHelper = DetachedObjectsHelper(Of Issue).Create(classInfo.KeyProperty.Name, properties)
        End Using
        Dim source = New InfiniteAsyncSource With {
            .CustomProperties = _DetachedObjectsHelper.Properties,
            .KeyProperty = nameof(Issue.Oid)
        }
        AddHandler source.FetchRows, AddressOf OnFetchRows
        AddHandler source.GetTotalSummaries, AddressOf OnGetTotalSummaries
        grid.ItemsSource = source
        LoadLookupData()
    End Sub

    Private Function MakeFilterExpression(ByVal filter As DevExpress.Data.Filtering.CriteriaOperator) As System.Linq.Expressions.Expression(Of System.Func(Of Issue, Boolean))
        Dim converter = New DevExpress.Xpf.Data.GridFilterCriteriaToExpressionConverter(Of Issue)()
        Return converter.Convert(filter)
    End Function
    Private _DetachedObjectsHelper As DetachedObjectsHelper(Of Issue)

    Private Sub OnFetchRows(ByVal sender As Object, ByVal e As FetchRowsAsyncEventArgs)
        e.Result = Task.Run(Of DevExpress.Xpf.Data.FetchRowsResult)(Function()
                                                                        Using session = New Session()
                                                                            Dim queryable = session.Query(Of Issue)().SortBy(e.SortOrder, defaultUniqueSortPropertyName:=NameOf(Issue.Oid)).Where(MakeFilterExpression(e.Filter))
                                                                            Dim items = queryable.Skip(e.Skip).Take(If(e.Take, 100)).ToArray()
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

    Private Sub OnValidateRowDeletion(ByVal sender As Object, ByVal e As GridValidateRowDeletionEventArgs)
        Using unitOfWork = New UnitOfWork()
            Dim key = _DetachedObjectsHelper.GetKey(e.Rows.[Single]())
            Dim item = unitOfWork.GetObjectByKey(Of Issue)(key)
            unitOfWork.Delete(item)
            unitOfWork.CommitChanges()
        End Using
    End Sub

    Private Sub LoadLookupData()
        Dim session = New DevExpress.Xpo.Session()
        usersLookup.ItemsSource = session.Query(Of XPOIssues.Issues.User).OrderBy(Function(user) user.Oid).[Select](Function(user) New With {
            .Id = user.Oid,
            .Name = user.FirstName & " " + user.LastName
        }).ToArray()
    End Sub

    Private Sub OnDataSourceRefresh(ByVal sender As Object, ByVal e As DevExpress.Xpf.Grid.DataSourceRefreshEventArgs)
        LoadLookupData()
    End Sub

End Class
