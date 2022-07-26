Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Grid
Imports DevExpress.CRUD.Northwind
Imports DevExpress.CRUD.Northwind.DataModel
Imports System
Imports System.Linq
Imports System.Windows
Imports System.Windows.Input

Namespace GridControlCRUDSimple

    Public Partial Class MainWindow
        Inherits DevExpress.Xpf.Core.ThemedWindow

        Public Sub New()
            Me.InitializeComponent()
            Using context = New DevExpress.CRUD.Northwind.NorthwindContext()
                Me.grid.ItemsSource = context.Products.[Select](Function(product) New DevExpress.CRUD.Northwind.DataModel.ProductInfo With {.Id = product.Id, .Name = product.Name, .CategoryId = product.CategoryId}).ToList()
                Me.categoriesLookup.ItemsSource = context.Categories.[Select](Function(category) New DevExpress.CRUD.Northwind.DataModel.CategoryInfo With {.Id = category.Id, .Name = category.Name}).ToList()
            End Using
        End Sub

        Private Sub tableView_ValidateRow(ByVal sender As Object, ByVal e As DevExpress.Xpf.Grid.GridRowValidationEventArgs)
            Dim productInfo = CType(e.Row, DevExpress.CRUD.Northwind.DataModel.ProductInfo)
            Using context = New DevExpress.CRUD.Northwind.NorthwindContext()
                Dim product As DevExpress.CRUD.Northwind.Product
                If Me.view.FocusedRowHandle = DevExpress.Xpf.Grid.DataControlBase.NewItemRowHandle Then
                    product = New DevExpress.CRUD.Northwind.Product()
                    context.Products.Add(product)
                Else
                    product = context.Products.SingleOrDefault(Function(p) p.Id = productInfo.Id)
                    If product Is Nothing Then
                        Throw New System.NotImplementedException("The modified row no longer exists in the database. Handle this case according to your requirements.")
                    End If
                End If

                product.Name = productInfo.Name
                product.CategoryId = productInfo.CategoryId
                context.SaveChanges()
                If Me.view.FocusedRowHandle = DevExpress.Xpf.Grid.DataControlBase.NewItemRowHandle Then
                    productInfo.Id = product.Id
                End If
            End Using
        End Sub

        Private Sub grid_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs)
            If e.Key = System.Windows.Input.Key.Delete Then
                Dim productInfo = CType(Me.grid.SelectedItem, DevExpress.CRUD.Northwind.DataModel.ProductInfo)
                If productInfo Is Nothing Then Return
                If DevExpress.Xpf.Core.DXMessageBox.Show(Me, "Are you sure you want to delete this row?", "Delete Row", System.Windows.MessageBoxButton.OKCancel) = System.Windows.MessageBoxResult.Cancel Then Return
                Try
                    Using context = New DevExpress.CRUD.Northwind.NorthwindContext()
                        Dim result = context.Products.Find(productInfo.Id)
                        If result Is Nothing Then
                            Throw New System.NotImplementedException("The modified row no longer exists in the database. Handle this case according to your requirements.")
                        End If

                        context.Products.Remove(result)
                        context.SaveChanges()
                        Me.view.Commands.DeleteFocusedRow.Execute(Nothing)
                    End Using
                Catch ex As System.Exception
                    Call DevExpress.Xpf.Core.DXMessageBox.Show(ex.Message)
                End Try
            End If
        End Sub
    End Class
End Namespace
