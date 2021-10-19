Imports DevExpress.Mvvm
Imports DevExpress.Xpf.Data
Imports System.Linq
Imports System.Threading.Tasks
Imports DevExpress.Xpo
Imports XPOIssues.Issues
Imports DevExpress.Mvvm.Xpf
Imports System

Public Class MainViewModel
    Inherits ViewModelBase
    Private _ServerModeSource As DevExpress.Xpo.XPServerModeView

    Public ReadOnly Property ServerModeSource As DevExpress.Xpo.XPServerModeView
        Get
            If _ServerModeSource Is Nothing Then
                Dim properties = New DevExpress.Xpo.ServerViewProperty() {
            New DevExpress.Xpo.ServerViewProperty("Oid", DevExpress.Xpo.SortDirection.Ascending, New DevExpress.Data.Filtering.OperandProperty("Oid")),
            New DevExpress.Xpo.ServerViewProperty("Subject", DevExpress.Xpo.SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Subject")),
            New DevExpress.Xpo.ServerViewProperty("UserId", DevExpress.Xpo.SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("UserId")),
            New DevExpress.Xpo.ServerViewProperty("Created", DevExpress.Xpo.SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Created")),
            New DevExpress.Xpo.ServerViewProperty("Votes", DevExpress.Xpo.SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Votes")),
            New DevExpress.Xpo.ServerViewProperty("Priority", DevExpress.Xpo.SortDirection.None, New DevExpress.Data.Filtering.OperandProperty("Priority"))
                }
                Dim session = New DevExpress.Xpo.Session()
                _ServerModeSource = New DevExpress.Xpo.XPServerModeView(session, GetType(Issues.Issue), Nothing)
                _ServerModeSource.Properties.AddRange(properties)
            End If

            Return _ServerModeSource
        End Get
    End Property
    Private _Users As System.Collections.IList

    Public ReadOnly Property Users As System.Collections.IList
        Get
            If _Users Is Nothing AndAlso Not IsInDesignMode Then
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
        Dim item = If(args.Key IsNot Nothing, unitOfWork.GetObjectByKey(Of Issue)(args.Key), New Issue(unitOfWork))
        args.ViewModel = New EditItemViewModel(item, New EditIssueInfo(unitOfWork, Users), dispose:=Sub() unitOfWork.Dispose())
    End Sub
    <DevExpress.Mvvm.DataAnnotations.Command>
    Public Sub ValidateRow(ByVal args As DevExpress.Mvvm.Xpf.EditFormRowValidationArgs)
        Dim unitOfWork = CType(args.Tag, EditIssueInfo).UnitOfWork
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