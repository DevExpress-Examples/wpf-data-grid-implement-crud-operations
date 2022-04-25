Imports DevExpress.Mvvm
Imports XPOIssues.Issues
Imports DevExpress.Xpo
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Xpf.Data
Imports System.Linq
Imports System.Threading.Tasks
Imports DevExpress.Mvvm.Xpf

Public Class MainViewModel
    Inherits ViewModelBase
    Private _DetachedObjectsHelper As DetachedObjectsHelper(Of Issue)
    Public ReadOnly Property DetachedObjectsHelper As DetachedObjectsHelper(Of Issue)
        Get
            If _DetachedObjectsHelper Is Nothing Then
                Using session = New Session()
                    Dim classInfo = session.GetClassInfo(Of Issue)()
                    Dim properties = classInfo.Members.Where(Function(member) member.IsPublic AndAlso member.IsPersistent).[Select](Function(member) member.Name).ToArray()
                    _DetachedObjectsHelper = DevExpress.Xpf.Data.DetachedObjectsHelper(Of Issue).Create(classInfo.KeyProperty.Name, properties)
                End Using
            End If
            Return _DetachedObjectsHelper
        End Get
    End Property
    Public ReadOnly Property Properties As System.ComponentModel.PropertyDescriptorCollection
        Get
            Return DetachedObjectsHelper.Properties
        End Get
    End Property

    Private Function MakeFilterExpression(ByVal filter As DevExpress.Data.Filtering.CriteriaOperator) As System.Linq.Expressions.Expression(Of System.Func(Of Issue, Boolean))
        Dim converter = New DevExpress.Xpf.Data.GridFilterCriteriaToExpressionConverter(Of Issue)()
        Return converter.Convert(filter)
    End Function
    <Command>
    Public Sub FetchRows(ByVal args As FetchRowsAsyncArgs)
        args.Result = Task.Run(Of DevExpress.Xpf.Data.FetchRowsResult)(Function()
                                                                           Using session = New Session()
                                                                               Dim queryable = session.Query(Of Issue)().SortBy(args.SortOrder, defaultUniqueSortPropertyName:=NameOf(Issue.Oid)).Where(MakeFilterExpression(CType(args.Filter, DevExpress.Data.Filtering.CriteriaOperator)))
                                                                               Dim items = queryable.Skip(args.Skip).Take(If(args.Take, 100)).ToArray()
                                                                               Return DetachedObjectsHelper.ConvertToDetachedObjects(items)
                                                                           End Using
                                                                       End Function)
    End Sub
    <Command>
    Public Sub GetTotalSummaries(ByVal args As GetSummariesAsyncArgs)
        args.Result = Task.Run(Function()
                                   Using session = New Session()
                                       Return session.Query(Of Issue)().Where(MakeFilterExpression(CType(args.Filter, DevExpress.Data.Filtering.CriteriaOperator))).GetSummaries(args.Summaries)
                                   End Using
                               End Function)
    End Sub
    <Command>
    Public Sub ValidateRow(ByVal args As RowValidationArgs)
        Using unitOfWork = New UnitOfWork()
            Dim item = If(args.IsNewItem, New Issue(unitOfWork), unitOfWork.GetObjectByKey(Of Issue)(DetachedObjectsHelper.GetKey(args.Item)))
            DetachedObjectsHelper.ApplyProperties(item, args.Item)
            unitOfWork.CommitChanges()

            If args.IsNewItem Then
                DetachedObjectsHelper.SetKey(args.Item, item.Oid)
            End If
        End Using
    End Sub
    <Command>
    Public Sub ValidateRowDeletion(ByVal args As ValidateRowDeletionArgs)
        Using unitOfWork = New UnitOfWork()
            Dim key = DetachedObjectsHelper.GetKey(args.Items.[Single]())
            Dim item = unitOfWork.GetObjectByKey(Of Issue)(key)
            unitOfWork.Delete(item)
            unitOfWork.CommitChanges()
        End Using
    End Sub
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

End Class