Imports XPOIssues.Issues
Imports DevExpress.Data.Filtering
Imports DevExpress.Xpo
Imports System.Linq
Imports DevExpress.Xpf.Grid
Imports DevExpress.Mvvm.Xpf
Imports System
Imports System.Collections
Class MainWindow
    Public Sub New()
        InitializeComponent()
        Dim properties = New ServerViewProperty() {
        New ServerViewProperty("Subject", SortDirection.None, New OperandProperty("Subject")),
        New ServerViewProperty("UserId", SortDirection.None, New OperandProperty("UserId")),
        New ServerViewProperty("Created", SortDirection.None, New OperandProperty("Created")),
        New ServerViewProperty("Votes", SortDirection.None, New OperandProperty("Votes")),
        New ServerViewProperty("Priority", SortDirection.None, New OperandProperty("Priority")),
        New ServerViewProperty("Oid", SortDirection.Ascending, New OperandProperty("Oid"))
        }
        Dim source = New XPInstantFeedbackView(GetType(Issue), properties, Nothing)
        AddHandler source.ResolveSession, Sub(o, e) e.Session = New Session()
        grid.ItemsSource = source
        LoadLookupData()
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

    Private Sub OnCreateEditEntityViewModel(ByVal sender As Object, ByVal e As CreateEditItemViewModelArgs)
        Dim unitOfWork = New UnitOfWork()
        Dim item = If(e.IsNewItem, New Issue(unitOfWork), unitOfWork.GetObjectByKey(Of Issue)(e.Key))
        e.ViewModel = New EditItemViewModel(item, New EditIssueInfo(unitOfWork, CType(usersLookup.ItemsSource, IList)), dispose:=Sub() unitOfWork.Dispose(), title:=If(e.IsNewItem, "New ", "Edit ") & NameOf(Issue))
    End Sub

    Private Sub OnValidateRow(ByVal sender As Object, ByVal e As EditFormRowValidationArgs)
        Dim unitOfWork = CType(e.EditOperationContext, EditIssueInfo).UnitOfWork
        unitOfWork.CommitChanges()
    End Sub

    Private Sub OnValidateRowDeletion(ByVal sender As Object, ByVal e As EditFormValidateRowDeletionArgs)
        Using unitOfWork = New UnitOfWork()
            Dim key = CInt(e.Keys.[Single]())
            Dim item = unitOfWork.GetObjectByKey(Of Issue)(key)
            unitOfWork.Delete(item)
            unitOfWork.CommitChanges()
        End Using
    End Sub

End Class
