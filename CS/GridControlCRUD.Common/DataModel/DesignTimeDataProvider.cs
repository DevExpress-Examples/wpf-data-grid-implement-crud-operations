using DevExpress.Xpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExpress.CRUD.DataModel {
    public class DesignTimeDataProvider<T> : IDataProvider<T> where T : class {
        readonly Func<int, T> createEntity;
        readonly int count;

        public DesignTimeDataProvider(Func<int, T> createEntity, int count = 5) {
            this.createEntity = createEntity;
            this.count = count;
        }

        IList<T> IDataProvider<T>.Read() {
            return Enumerable.Range(0, count).Select(createEntity).ToList();
        }

        TResult IDataProvider<T>.GetQueryableResult<TResult>(Func<IQueryable<T>, TResult> getResult) {
            var queryable = ((IDataProvider<T>)this).Read().AsQueryable();
            return getResult(queryable);
        }

        void IDataProvider<T>.Create(T obj) {
            throw new NotSupportedException();
        }
        void IDataProvider<T>.Delete(T obj) {
            throw new NotSupportedException();
        }
        void IDataProvider<T>.Update(T obj) {
            throw new NotSupportedException();
        }

        string IDataProvider<T>.KeyProperty => throw new NotSupportedException();
    }
}
