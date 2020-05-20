using System;
using System.Linq;

namespace DevExpress.CRUD.DataModel {
    public interface ICRUDDataProvider<T> : IDataProvider<T> where T : class {
        void Create(T obj);
        void Update(T obj);
        void Delete(T obj);
    }
}
