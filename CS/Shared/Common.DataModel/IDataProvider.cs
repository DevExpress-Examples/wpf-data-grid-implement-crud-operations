using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExpress.CRUD.DataModel {
    public interface IDataProvider<T> where T : class {
        IList<T> Read();
    }
}
