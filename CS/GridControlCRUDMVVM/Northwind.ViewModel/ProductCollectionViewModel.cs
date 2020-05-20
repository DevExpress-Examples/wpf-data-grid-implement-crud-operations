using DevExpress.CRUD.DataModel;
using DevExpress.CRUD.Northwind.DataModel;
using DevExpress.CRUD.ViewModel;
using System.Collections.Generic;

namespace DevExpress.CRUD.Northwind.ViewModel {
    public class ProductCollectionViewModel : CollectionViewModel<ProductInfo> {
        public IList<CategoryInfo> Categories {
            get => GetValue<IList<CategoryInfo>>();
            private set => SetValue(value);
        }

        readonly IDataProvider<CategoryInfo> categoriesDataProvider;

        public ProductCollectionViewModel() 
            : this(NorthwindDataStorageFactory.Create(IsInDesignMode)) {
        }

        public ProductCollectionViewModel(NorthwindDataStorage dataStorage) 
            : base(dataStorage.Products) {
            categoriesDataProvider = dataStorage.Categories;
            OnRefreshCore();
        }

        protected override void OnRefreshCore() {
            if(categoriesDataProvider != null) {
                try {
                    Categories = categoriesDataProvider.Read();
                } catch {
                    Categories = null;
                }
            }
        }
    }
}
