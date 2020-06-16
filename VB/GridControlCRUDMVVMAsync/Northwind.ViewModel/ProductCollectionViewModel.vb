Imports DevExpress.CRUD.DataModel
Imports DevExpress.CRUD.Northwind.DataModel
Imports DevExpress.CRUD.ViewModel
Imports System.Collections.Generic
Imports System.Threading.Tasks

Namespace DevExpress.CRUD.Northwind.ViewModel
	Public Class ProductCollectionViewModel
		Inherits CollectionViewModel(Of ProductInfo)

		Public Property Categories() As IList(Of CategoryInfo)
			Get
				Return GetValue(Of IList(Of CategoryInfo))()
			End Get
			Private Set(ByVal value As IList(Of CategoryInfo))
				SetValue(value)
			End Set
		End Property

		Private ReadOnly categoriesDataProvider As IDataProvider(Of CategoryInfo)

		Public Sub New()
			Me.New(NorthwindDataStorageFactory.Create(IsInDesignMode))
		End Sub

		Public Sub New(ByVal dataStorage As NorthwindDataStorage)
			MyBase.New(dataStorage.Products)
			categoriesDataProvider = dataStorage.Categories
			RefreshCategories()
		End Sub

		Private Async Sub RefreshCategories()
			Await OnRefreshCoreAsync()
		End Sub
		Protected Overrides Async Function OnRefreshCoreAsync() As Task
			If categoriesDataProvider IsNot Nothing Then
				Try
					Categories = Await categoriesDataProvider.ReadAsync()
				Catch
					Categories = Nothing
				End Try
			End If
		End Function
	End Class
End Namespace
