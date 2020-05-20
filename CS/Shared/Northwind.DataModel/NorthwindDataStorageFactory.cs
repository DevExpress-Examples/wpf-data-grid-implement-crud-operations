using DevExpress.CRUD.DataModel.EntityFramework;
using DevExpress.CRUD.DataModel;

namespace DevExpress.CRUD.Northwind.DataModel {
    public static class NorthwindDataStorageFactory {
        public static NorthwindDataStorage Create(bool isInDesignMode) {
            if(isInDesignMode) { 
                return new NorthwindDataStorage(
                    new DesignTimeDataProvider<CategoryInfo>(
                        id => new CategoryInfo {
                            Id = id,
                            Name = "Category " + id,
                        }
                    ),
                    new DesignTimeDataProvider<ProductInfo>(
                        id => new ProductInfo {
                            Id = id,
                            Name = "Product " + id,
                            CategoryId = id
                        }
                    )
                );
            }
            return new NorthwindDataStorage(
                new EntityFrameworkDataProvider<NorthwindContext, Category, CategoryInfo>(
                    createContext: () => new NorthwindContext(),
                    getDbSet: context => context.Categories,
                    getEnityExpression: category => new CategoryInfo {
                        Id = category.Id,
                        Name = category.Name,
                    }
                ),
                new EntityFrameworkCRUDDataProvider<NorthwindContext, Product, ProductInfo, long>(
                    createContext: () => new NorthwindContext(),
                    getDbSet: context => context.Products,
                    getEnityExpression: product => new ProductInfo {
                        Id = product.Id,
                        Name = product.Name,
                        CategoryId = product.CategoryId
                    },
                    getKey: productInfo => productInfo.Id,
                    getEntityKey: product => product.Id,
                    setKey: (productInfo, id) => productInfo.Id = id,
                    applyProperties: (productInfo, product) => {
                        product.Name = productInfo.Name;
                        product.CategoryId = productInfo.CategoryId;
                    }
                )
            );
        }
    }
}
