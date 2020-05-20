using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExpress.CRUD.DataModel {
    public class DesignTimeDataProvider<T> : ICRUDDataProvider<T> where T : class {
        readonly Func<int, T> createEntity;
        readonly int count;

        public DesignTimeDataProvider(Func<int, T> createEntity, int count = 5) {
            this.createEntity = createEntity;
            this.count = count;
        }

        IList<T> IDataProvider<T>.Read() {
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
