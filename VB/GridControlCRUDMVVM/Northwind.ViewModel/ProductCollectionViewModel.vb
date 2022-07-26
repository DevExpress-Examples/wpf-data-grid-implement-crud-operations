Imports DevExpress.CRUD.DataModel
Imports DevExpress.CRUD.Northwind.DataModel
Imports DevExpress.CRUD.ViewModel
Imports System.Collections.Generic

Namespace DevExpress.CRUD.Northwind.ViewModel

    Public Class ProductCollectionViewModel
        Inherits CollectionViewModel(Of ProductInfo)

        Public Property Categories As IList(Of CategoryInfo)
            Get
                Return GetValue(Of IList(Of CategoryInfo))()
            End Get

            Private Set(ByVal value As IList(Of CategoryInfo))
                SetValue(value)
            End Set
        End Property

        Private ReadOnly categoriesDataProvider As IDataProvider(Of CategoryInfo)

        Public Sub New()
            Me.New(Create(IsInDesignMode))
        End Sub

        Public Sub New(ByVal dataStorage As NorthwindDataStorage)
            MyBase.New(dataStorage.Products)
            categoriesDataProvider = dataStorage.Categories
            OnRefreshCore()
        End Sub

        Protected Overrides Sub OnRefreshCore()
            If categoriesDataProvider IsNot Nothing Then
                Try
                    Categories = categoriesDataProvider.Read()
                Catch
                    Categories = Nothing
                End Try
            End If
        End Sub
    End Class
End Namespace
