Imports DevExpress.Mvvm
Imports XPOIssues.Issues
Imports DevExpress.Xpo
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Xpf.Data
Imports System.Linq
Imports System.Threading.Tasks
Imports DevExpress.Mvvm.Xpf
Imports System

Public Class MainViewModel
    Inherits ViewModelBase
    Private _ItemsSource As XPInstantFeedbackView
    Public ReadOnly Property ItemsSource As XPInstantFeedbackView
        Get
            If _ItemsSource Is Nothing Then
                Dim properties = New ServerViewProperty() {
            New ServerViewProperty("Subject", SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Subject")),
            New ServerViewProperty("UserId", SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("UserId")),
            New ServerViewProperty("Created", SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Created")),
            New ServerViewProperty("Votes", SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Votes")),
            New ServerViewProperty("Priority", SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Priority")),
            New ServerViewProperty("Oid", SortDirection.Ascending, New DevExpress.Data.Filtering.OperandProperty("Oid"))
                }
                _ItemsSource = New XPInstantFeedbackView(GetType(Issue), properties, Nothing)
                AddHandler _ItemsSource.ResolveSession, Sub(o, e) e.Session = New Session()
            End If
            Return _ItemsSource
        End Get
    End Property
    Private _Users As System.Collections.IList
    Public ReadOnly Property Users As System.Collections.IList
        Get
            If _Users Is Nothing AndAlso Not DevExpress.Mvvm.ViewModelBase.IsInDesignMode Then
                Dim session = New DevExpress.Xpo.Session()
                _Users = session.Query(Of XPOIssues.Issues.User).OrderBy(Function(user) user.Oid).[Select](Function(user) New With {
                    .Id = user.Oid,
                    .Name = user.FirstName & " " + user.LastName
                }).ToArray()
            End If
            Return _Users
        End Get
    End Property
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub DataSourceRefresh(ByVal args As DevExpress.Mvvm.Xpf.DataSourceRefreshArgs)
        _Users = Nothing
        RaisePropertyChanged(Nameof(Users))
    End Sub
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub CreateEditEntityViewModel(ByVal args As DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs)
        Dim unitOfWork = New UnitOfWork()
        Dim item = If(args.IsNewItem, New Issue(unitOfWork), unitOfWork.GetObjectByKey(Of Issue)(args.Key))
        args.ViewModel = New EditItemViewModel(item, New EditIssueInfo(unitOfWork, Users), dispose:=Sub() unitOfWork.Dispose(), title:=If(args.IsNewItem, "New ", "Edit ") & NameOf(Issue))
    End Sub
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub ValidateRow(ByVal args As DevExpress.Mvvm.Xpf.EditFormRowValidationArgs)
        Dim unitOfWork = CType(args.EditOperationContext, EditIssueInfo).UnitOfWork
        unitOfWork.CommitChanges()
    End Sub
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub ValidateRowDeletion(ByVal args As DevExpress.Mvvm.Xpf.EditFormValidateRowDeletionArgs)
        Using unitOfWork = New UnitOfWork()
            Dim key = CInt(args.Keys.[Single]())
            Dim item = unitOfWork.GetObjectByKey(Of Issue)(key)
            unitOfWork.Delete(item)
            unitOfWork.CommitChanges()
        End Using
    End Sub

End Class