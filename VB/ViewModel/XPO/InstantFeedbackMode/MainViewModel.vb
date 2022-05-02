Imports DevExpress.Mvvm
Imports XPOIssues.Issues
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Data.Filtering
Imports DevExpress.Xpo
Imports System.Linq
Imports System.Collections
Imports DevExpress.Mvvm.Xpf
Imports System

Public Class MainViewModel
    Inherits ViewModelBase
    Private _ItemsSource As XPInstantFeedbackView
    Public ReadOnly Property ItemsSource As XPInstantFeedbackView
        Get
            If _ItemsSource Is Nothing Then
                Dim properties = New ServerViewProperty() {
            New ServerViewProperty("Subject", SortDirection.None, New OperandProperty("Subject")),
            New ServerViewProperty("UserId", SortDirection.None, New OperandProperty("UserId")),
            New ServerViewProperty("Created", SortDirection.None, New OperandProperty("Created")),
            New ServerViewProperty("Votes", SortDirection.None, New OperandProperty("Votes")),
            New ServerViewProperty("Priority", SortDirection.None, New OperandProperty("Priority")),
            New ServerViewProperty("Oid", SortDirection.Ascending, New OperandProperty("Oid"))
                }
                _ItemsSource = New XPInstantFeedbackView(GetType(Issue), properties, Nothing)
                AddHandler _ItemsSource.ResolveSession, Sub(o, e) e.Session = New Session()
            End If
            Return _ItemsSource
        End Get
    End Property
    Private _Users As IList
    Public ReadOnly Property Users As IList
        Get
            If _Users Is Nothing AndAlso Not DevExpress.Mvvm.ViewModelBase.IsInDesignMode Then
                Dim session = New Session()
                _Users = session.Query(Of User).OrderBy(Function(user) user.Oid).[Select](Function(user) New With {
                    .Id = user.Oid,
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
        Dim unitOfWork = New UnitOfWork()
        Dim item = If(args.IsNewItem, New Issue(unitOfWork), unitOfWork.GetObjectByKey(Of Issue)(args.Key))
        args.ViewModel = New EditItemViewModel(item, New EditIssueInfo(unitOfWork, Users), dispose:=Sub() unitOfWork.Dispose(), title:=If(args.IsNewItem, "New ", "Edit ") & NameOf(Issue))
    End Sub
    <Command>
    Public Sub ValidateRow(ByVal args As EditFormRowValidationArgs)
        Dim unitOfWork = CType(args.EditOperationContext, EditIssueInfo).UnitOfWork
        unitOfWork.CommitChanges()
    End Sub
    <Command>
    Public Sub ValidateRowDeletion(ByVal args As EditFormValidateRowDeletionArgs)
        Using unitOfWork = New UnitOfWork()
            Dim key = CInt(args.Keys.[Single]())
            Dim item = unitOfWork.GetObjectByKey(Of Issue)(key)
            unitOfWork.Delete(item)
            unitOfWork.CommitChanges()
        End Using
    End Sub

End Class