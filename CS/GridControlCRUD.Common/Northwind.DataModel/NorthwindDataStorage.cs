using DevExpress.CRUD.DataModel;

namespace DevExpress.CRUD.Northwind.DataModel {
    public class NorthwindDataStorage {
        public NorthwindDataStorage(IDataProvider<CategoryInfo> categories, IDataProvider<ProductInfo> products) {
            Categories = categories;
            Products = products;
        }
        public IDataProvider<CategoryInfo> Categories { get; }
        public IDataProvider<ProductInfo> Products { get; }
    }
}
