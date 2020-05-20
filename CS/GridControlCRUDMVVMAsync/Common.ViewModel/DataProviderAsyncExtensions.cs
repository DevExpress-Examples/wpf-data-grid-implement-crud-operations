using DevExpress.CRUD.DataModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevExpress.CRUD.ViewModel {
    public static class DataProviderAsyncExtensions {
        public static async Task<IList<T>> ReadAsync<T>(this IDataProvider<T> dataProvider) where T : class {
#if DEBUG
            await Task.Delay(500);
#endif
            return await Task.Run(dataProvider.Read);
        }
        public static async Task UpdateAsync<T>(this ICRUDDataProvider<T> dataProvider, T entity) where T : class {
#if DEBUG
            await Task.Delay(500);
#endif
            await Task.Run(() => dataProvider.Update(entity));
        }
        public static async Task CreateAsync<T>(this ICRUDDataProvider<T> dataProvider, T entity) where T : class {
#if DEBUG
            await Task.Delay(500);
#endif
            await Task.Run(() => dataProvider.Create(entity));
        }
    }
}
