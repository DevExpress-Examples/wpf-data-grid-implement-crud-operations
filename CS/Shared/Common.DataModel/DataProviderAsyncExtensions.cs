using DevExpress.CRUD.DataModel;
using DevExpress.Xpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExpress.CRUD.ViewModel {
    public static class DataProviderAsyncExtensions {
        public static async Task<IList<T>> ReadAsync<T>(this IDataProvider<T> dataProvider) where T : class {
#if DEBUG
            await Task.Delay(500);
#endif
            return await Task.Run(() => dataProvider.Read());
        }
        public static async Task<TResult> GetQueryableResultAsync<T, TResult>(this IDataProvider<T> dataProvider, Func<IQueryable<T>, TResult> getResult) where T : class {
#if DEBUG
            await Task.Delay(500);
#endif
            return await Task.Run(() => dataProvider.GetQueryableResult(getResult));
        }
        public static async Task UpdateAsync<T>(this IDataProvider<T> dataProvider, T entity) where T : class {
#if DEBUG
            await Task.Delay(500);
#endif
            await Task.Run(() => dataProvider.Update(entity));
        }
        public static async Task CreateAsync<T>(this IDataProvider<T> dataProvider, T entity) where T : class {
#if DEBUG
            await Task.Delay(500);
#endif
            await Task.Run(() => dataProvider.Create(entity));
        }
    }
}
