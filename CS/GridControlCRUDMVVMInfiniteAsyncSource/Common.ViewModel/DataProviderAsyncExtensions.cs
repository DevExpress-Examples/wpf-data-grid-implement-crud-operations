using DevExpress.CRUD.DataModel;
using DevExpress.Xpf.Data;
using System;
using System.Collections.Generic;
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

        public static async Task<IList<T>> FetchAsync<T>(this IDataProvider<T> dataProvider, SortDefinition[] sortOrder, Expression<Func<T, bool>> filter, int skip, int take) where T : class {
#if DEBUG
            await Task.Delay(500);
#endif
            return await Task.Run(() => dataProvider.Fetch(sortOrder, filter, skip, take));
        }
        public static async Task<object[]> GetTotalSummariesAsync<T>(this IDataProvider<T> dataProvider, SummaryDefinition[] summaries, Expression<Func<T, bool>> filter) where T : class {
#if DEBUG
            await Task.Delay(500);
#endif
            return await Task.Run(() => dataProvider.GetTotalSummaries(summaries, filter));
        }
        public static async Task<ValueAndCount[]> GetDistinctValuesAsync<T>(this IDataProvider<T> dataProvider, string propertyName, Expression<Func<T, bool>> filter) where T : class {
#if DEBUG
            await Task.Delay(500);
#endif
            return await Task.Run(() => dataProvider.GetDistinctValues(propertyName, filter));
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
