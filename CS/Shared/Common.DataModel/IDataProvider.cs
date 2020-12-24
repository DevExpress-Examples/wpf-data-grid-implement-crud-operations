using DevExpress.Xpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExpress.CRUD.DataModel {
    public interface IDataProvider<T> where T : class {
        IList<T> Read();
        IList<T> Fetch(SortDefinition[] sortOrder, Expression<Func<T, bool>> filter, int skip, int take);
        object[] GetTotalSummaries(SummaryDefinition[] summaries, Expression<Func<T, bool>> filter);
        ValueAndCount[] GetDistinctValues(string propertyName, Expression<Func<T, bool>> filter);
    }
}
