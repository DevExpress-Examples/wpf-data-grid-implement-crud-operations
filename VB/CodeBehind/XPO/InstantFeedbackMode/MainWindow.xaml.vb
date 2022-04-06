Imports XPOIssues.Issues
Imports DevExpress.Xpo
Imports DevExpress.Xpf.Data
Imports System.Linq
Imports System.Threading.Tasks
Imports DevExpress.Mvvm.Xpf
Imports System
Imports System.Collections
Class MainWindow
    Public Sub New()
        InitializeComponent()
        Dim properties = New ServerViewProperty() {
        New ServerViewProperty("Subject", SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Subject")),
        New ServerViewProperty("UserId", SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("UserId")),
        New ServerViewProperty("Created", SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Created")),
        New ServerViewProperty("Votes", SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Votes")),
        New ServerViewProperty("Priority", SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Priority")),
        New ServerViewProperty("Oid", SortDirection.Ascending, New DevExpress.Data.Filtering.OperandProperty("Oid"))
        }
        Dim source = New XPInstantFeedbackView(GetType(Issue), properties, Nothing)
        AddHandler source.ResolveSession, Sub(o, e) e.Session = New Session()
        grid.ItemsSource = source
        LoadLookupData()
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

    Private Sub OnCreateEditEntityViewModel(ByVal sender As Object, ByVal e As DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs)
        Dim unitOfWork = New UnitOfWork()
        Dim item = If(e.IsNewItem, New Issue(unitOfWork), unitOfWork.GetObjectByKey(Of Issue)(e.Key))
        e.ViewModel = New EditItemViewModel(item, New EditIssueInfo(unitOfWork, CType(usersLookup.ItemsSource, IList)), dispose:=Sub() unitOfWork.Dispose(), title:=If(e.IsNewItem, "New ", "Edit ") & NameOf(Issue))
    End Sub

    Private Sub OnValidateRow(ByVal sender As Object, ByVal e As DevExpress.Mvvm.Xpf.EditFormRowValidationArgs)
        Dim unitOfWork = CType(e.EditOperationContext, EditIssueInfo).UnitOfWork
        unitOfWork.CommitChanges()
    End Sub

    Private Sub OnValidateRowDeletion(ByVal sender As Object, ByVal e As DevExpress.Mvvm.Xpf.EditFormValidateRowDeletionArgs)
        Using unitOfWork = New UnitOfWork()
            Dim key = CInt(e.Keys.[Single]())
            Dim item = unitOfWork.GetObjectByKey(Of Issue)(key)
            unitOfWork.Delete(item)
            unitOfWork.CommitChanges()
        End Using
    End Sub

End Class
