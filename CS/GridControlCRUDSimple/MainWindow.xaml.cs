using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.CRUD.Northwind;
using System;
using System.Linq;
using System.Windows;
using DevExpress.CRUD.Northwind.DataModel;

namespace GridControlCRUDSimple {
    public partial class MainWindow : ThemedWindow {
        public MainWindow() {
            InitializeComponent();
            using(var context = new NorthwindContext()) {
                grid.ItemsSource = context
                    .Products
                    .Select(product => new ProductInfo {
                        Id = product.Id,
                        Name = product.Name,
                        CategoryId = product.CategoryId
                    })
                    .ToList();
                categoriesLookup.ItemsSource = context
                    .Categories
                    .Select(category => new CategoryInfo {
                        Id = category.Id,
                        Name = category.Name,
                    })
                    .ToList();
            }
        }

        void OnValidateRow(object sender, GridRowValidationEventArgs e) {
            var productInfo = (ProductInfo)e.Row;
            using(var context = new NorthwindContext()) {
                Product product;
                if(view.FocusedRowHandle == DataControlBase.NewItemRowHandle) {
                    product = new Product();
                    context.Products.Add(product);
                } else {
                    product = context.Products.SingleOrDefault(p => p.Id == productInfo.Id);
                    if(product == null) {
                        throw new NotImplementedException("The modified row no longer exists in the database. Handle this case according to your requirements.");
                    }
                }
                product.Name = productInfo.Name;
                product.CategoryId = productInfo.CategoryId;
                context.SaveChanges();
                if(view.FocusedRowHandle == DataControlBase.NewItemRowHandle) {
                    productInfo.Id = product.Id;
                }
            }
        }

        void OnValidateDeleteRows(object sender, GridDeleteRowsValidationEventArgs e) {
            var productInfo = (ProductInfo)e.Rows[0];
            if(DXMessageBox.Show(this, "Are you sure you want to delete this row?", "Delete Row", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) {
                e.Result = "Canceled";
                return;
            }
            using(var context = new NorthwindContext()) {
                var result = context.Products.Find(productInfo.Id);
                if(result == null) {
                    throw new NotImplementedException("The modified row no longer exists in the database. Handle this case according to your requirements.");
                }
                context.Products.Remove(result);
                context.SaveChanges();
            }
        }
    }
}
