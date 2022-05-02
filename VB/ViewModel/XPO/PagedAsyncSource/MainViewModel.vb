Imports DevExpress.Mvvm
Imports XPOIssues.Issues
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Mvvm.Xpf
Imports System
Imports System.Linq.Expressions
Imports DevExpress.Data.Filtering
Imports DevExpress.Xpf.Data
Imports System.Linq
Imports System.Threading.Tasks
Imports DevExpress.Xpo
Imports System.ComponentModel
Imports System.Collections

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
    Public ReadOnly Property Properties As PropertyDescriptorCollection
        Get
            Return DetachedObjectsHelper.Properties
        End Get
    End Property

    Private Function MakeFilterExpression(ByVal filter As CriteriaOperator) As Expression(Of Func(Of Issue, Boolean))
        Dim converter = New GridFilterCriteriaToExpressionConverter(Of Issue)()
        Return converter.Convert(filter)
    End Function
    <Command>
    Public Sub FetchPage(ByVal args As FetchPageAsyncArgs)
        args.Result = Task.Run(Of FetchRowsResult)(Function()
                                                       Const pageTakeCount As Integer = 5

                                                       Using session = New Session()
                                                           Dim queryable = session.Query(Of Issue)().SortBy(args.SortOrder, defaultUniqueSortPropertyName:=NameOf(Issue.Oid)).Where(MakeFilterExpression(CType(args.Filter, CriteriaOperator)))
                                                           Dim items = queryable.Skip(args.Skip).Take(args.Take * pageTakeCount).ToArray()
                                                           Return DetachedObjectsHelper.ConvertToDetachedObjects(items)
                                                       End Using
                                                   End Function)
    End Sub
    <Command>
    Public Sub GetTotalSummaries(ByVal args As GetSummariesAsyncArgs)
        args.Result = Task.Run(Function()
                                   Using session = New Session()
                                       Return session.Query(Of Issue)().Where(MakeFilterExpression(CType(args.Filter, CriteriaOperator))).GetSummaries(args.Summaries)
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

End Class