using DevExpress.Xpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExpress.CRUD.DataModel {
    public class DesignTimeDataProvider<T> : ICRUDDataProvider<T> where T : class {
        readonly Func<int, T> createEntity;
        readonly int count;

        public DesignTimeDataProvider(Func<int, T> createEntity, int count = 5) {
            this.createEntity = createEntity;
            this.count = count;
        }

        IList<T> IDataProvider<T>.Read() {
            return ReadCore();
        }

        IList<T> IDataProvider<T>.Fetch(SortDefinition[] sortOrder, Expression<Func<T, bool>> filter, int skip, int take) {
            return ReadCore();
        }
        object[] IDataProvider<T>.GetTotalSummaries(SummaryDefinition[] summaries, Expression<Func<T, bool>> filter) {
            return Enumerable.Repeat(default(object), summaries.Length).ToArray();
        }
        ValueAndCount[] IDataProvider<T>.GetDistinctValues(string propertyName, Expression<Func<T, bool>> filter) {
            return new ValueAndCount[0];
        }

        IList<T> ReadCore() {
            return Enumerable.Range(0, count).Select(createEntity).ToList();
        }

        void ICRUDDataProvider<T>.Create(T obj) {
            throw new NotSupportedException();
        }
        void ICRUDDataProvider<T>.Delete(T obj) {
            throw new NotSupportedException();
        }
        void ICRUDDataProvider<T>.Update(T obj) {
            throw new NotSupportedException();
        }
    }
}
