Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Grid
Imports DevExpress.CRUD.Northwind
Imports DevExpress.CRUD.Northwind.DataModel
Imports System
Imports System.Linq
Imports System.Windows
Imports System.Windows.Input

Namespace GridControlCRUDSimple
	Partial Public Class MainWindow
		Inherits ThemedWindow

		Public Sub New()
			InitializeComponent()
			Using context = New NorthwindContext()
				grid.ItemsSource = context.Products.Select(Function(product) New ProductInfo With {
					.Id = product.Id,
					.Name = product.Name,
					.CategoryId = product.CategoryId
				}).ToList()
				categoriesLookup.ItemsSource = context.Categories.Select(Function(category) New CategoryInfo With {
					.Id = category.Id,
					.Name = category.Name
				}).ToList()
			End Using
		End Sub

		Private Sub tableView_ValidateRow(ByVal sender As Object, ByVal e As GridRowValidationEventArgs)
			Dim productInfo As ProductInfo = CType(e.Row, ProductInfo)
			Using context = New NorthwindContext()
				Dim result As Product
				If view.FocusedRowHandle = GridControl.NewItemRowHandle Then
					result = New Product()
					context.Products.Add(result)
				Else
					result = context.Products.SingleOrDefault(Function(product) product.Id = productInfo.Id)
					If result Is Nothing Then
						Throw New NotImplementedException("The modified row does not exist in a database anymore. Handle this case according to your requirements.")
					End If
				End If
				result.Name = productInfo.Name
				result.CategoryId = productInfo.CategoryId
				context.SaveChanges()
			End Using
		End Sub

		Private Sub grid_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
			If e.Key = Key.Delete Then
				Dim productInfo As ProductInfo = CType(grid.SelectedItem, ProductInfo)
				If productInfo Is Nothing Then
					Return
				End If
				If DXMessageBox.Show(Me, "Are you sure you want to delete this row?", "Delete Row", MessageBoxButton.OKCancel) = MessageBoxResult.Cancel Then
					Return
				End If
				Try
					Using context = New NorthwindContext()
						Dim result = context.Products.Find(productInfo.Id)
						If result Is Nothing Then
							Throw New NotImplementedException("The deleted row does not exist in a database anymore. Handle this case according to your requirements.")
						End If
						context.Products.Remove(result)
						context.SaveChanges()
						view.Commands.DeleteFocusedRow.Execute(Nothing)
					End Using
				Catch ex As Exception
					DXMessageBox.Show(ex.Message)
				End Try
			End If
		End Sub
	End Class
End Namespace
