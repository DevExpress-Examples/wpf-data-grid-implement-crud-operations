using DevExpress.Xpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExpress.CRUD.DataModel {
    public interface IDataProvider<T> where T : class {
        IList<T> Read();
        void Create(T obj);
        void Update(T obj);
        void Delete(T obj);
        TResult GetQueryableResult<TResult>(Func<IQueryable<T>, TResult> getResult); //used for virtual sources
        string KeyProperty { get; }
    }
}
