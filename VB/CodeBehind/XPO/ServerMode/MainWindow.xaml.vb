Imports DevExpress.Xpf.Data
Imports System.Linq
Imports System.Threading.Tasks
Imports DevExpress.Xpo
Imports XPOIssues.Issues
Imports DevExpress.Mvvm.Xpf
Imports System
Imports System.Collections
Class MainWindow
    Public Sub New()
        InitializeComponent()
        Dim properties = New DevExpress.Xpo.ServerViewProperty() {
        New DevExpress.Xpo.ServerViewProperty("Oid", DevExpress.Xpo.SortDirection.Ascending, New DevExpress.Data.Filtering.OperandProperty("Oid")),
        New DevExpress.Xpo.ServerViewProperty("Subject", DevExpress.Xpo.SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Subject")),
        New DevExpress.Xpo.ServerViewProperty("UserId", DevExpress.Xpo.SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("UserId")),
        New DevExpress.Xpo.ServerViewProperty("Created", DevExpress.Xpo.SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Created")),
        New DevExpress.Xpo.ServerViewProperty("Votes", DevExpress.Xpo.SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Votes")),
        New DevExpress.Xpo.ServerViewProperty("Priority", DevExpress.Xpo.SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Priority"))
        }
        Dim session = New DevExpress.Xpo.Session()
        Dim source = New DevExpress.Xpo.XPServerModeView(session, GetType(Issues.Issue), Nothing)
        source.Properties.AddRange(properties)
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

    Private Sub OnDataSourceRefresh(ByVal sender As System.Object, ByVal e As DevExpress.Xpf.Grid.DataSourceRefreshEventArgs)
        LoadLookupData()
    End Sub

    Private Sub OnCreateEditEntityViewModel(ByVal sender As System.Object, ByVal e As DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs)
        Dim unitOfWork = New UnitOfWork()
        Dim item = If(e.Key IsNot Nothing, unitOfWork.GetObjectByKey(Of Issue)(e.Key), New Issue(unitOfWork))
        e.ViewModel = New EditItemViewModel(item, New EditIssueInfo(unitOfWork, CType(usersLookup.ItemsSource, IList)), dispose:=Sub() unitOfWork.Dispose())
    End Sub

    Private Sub OnValidateRow(ByVal sender As System.Object, ByVal e As DevExpress.Mvvm.Xpf.EditFormRowValidationArgs)
        Dim unitOfWork = CType(e.EditOperationContext, EditIssueInfo).UnitOfWork
        unitOfWork.CommitChanges()
    End Sub

    Private Sub OnValidateRowDeletion(ByVal sender As System.Object, ByVal e As DevExpress.Mvvm.Xpf.EditFormValidateRowDeletionArgs)
        Using unitOfWork = New UnitOfWork()
            Dim key = CInt(e.Keys.[Single]())
            Dim item = unitOfWork.GetObjectByKey(Of Issue)(key)
            unitOfWork.Delete(item)
            unitOfWork.CommitChanges()
        End Using
    End Sub

End Class
