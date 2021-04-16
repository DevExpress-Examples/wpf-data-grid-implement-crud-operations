Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Grid
Imports DevExpress.CRUD.Northwind
Imports System
Imports System.Linq
Imports System.Windows
Imports System.Windows.Input
Imports DevExpress.CRUD.Northwind.DataModel

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
				Dim product As Product
				If view.FocusedRowHandle = DataControlBase.NewItemRowHandle Then
					product = New Product()
					context.Products.Add(product)
				Else
					product = context.Products.SingleOrDefault(Function(p) p.Id = productInfo.Id)
					If product Is Nothing Then
						Throw New NotImplementedException("The modified row no longer exists in the database. Handle this case according to your requirements.")
					End If
				End If
				product.Name = productInfo.Name
				product.CategoryId = productInfo.CategoryId
				context.SaveChanges()
				If view.FocusedRowHandle = DataControlBase.NewItemRowHandle Then
					productInfo.Id = product.Id
				End If
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
							Throw New NotImplementedException("The modified row no longer exists in the database. Handle this case according to your requirements.")
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
