using DevExpress.CRUD.DataModel;
using DevExpress.CRUD.Northwind.DataModel;
using DevExpress.CRUD.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            RefreshCategories();
        }

        async void RefreshCategories() {
            await OnRefreshCoreAsync();
        }
        protected override async Task OnRefreshCoreAsync() {
            if(categoriesDataProvider != null) {
                try {
                    Categories = await categoriesDataProvider.ReadAsync();
                } catch {
                    Categories = null;
                }
            }
        }
    }
}
