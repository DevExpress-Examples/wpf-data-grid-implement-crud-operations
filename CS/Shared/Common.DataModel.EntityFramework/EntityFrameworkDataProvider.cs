using DevExpress.Xpf.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DevExpress.CRUD.DataModel.EntityFramework {
    public class EntityFrameworkDataProvider<TContext, TEntity, T> : IDataProvider<T>
        where TEntity : class
        where T : class
        where TContext : DbContext {

        protected readonly Func<TContext> createContext;
        protected readonly Func<TContext, DbSet<TEntity>> getDbSet;
        readonly Expression<Func<TEntity, T>> getEntityExpression;
        readonly string keyProperty;

        public EntityFrameworkDataProvider(Func<TContext> createContext, Func<TContext, DbSet<TEntity>> getDbSet, Expression<Func<TEntity, T>> getEnityExpression, string keyProperty) {
            this.createContext = createContext;
            this.getDbSet = getDbSet;
            this.getEntityExpression = getEnityExpression;
            this.keyProperty = keyProperty;
        }

        IList<T> IDataProvider<T>.Read() {
            using(var context = createContext()) {
                var query = getDbSet(context)
                    .Select(getEntityExpression);
                return query.ToList();
            }
        }

        IList<T> IDataProvider<T>.Fetch(SortDefinition[] sortOrder, Expression<Func<T, bool>> filter, int skip, int take) {
            using(var context = createContext()) {
                var query = getDbSet(context)
                    .Select(getEntityExpression)
                    .SortBy(sortOrder, defaultUniqueSortPropertyName: keyProperty)
                    .Where(filter)
                    .Skip(skip)
                    .Take(take);
                return query.ToList();
            }
        }
        object[] IDataProvider<T>.GetTotalSummaries(SummaryDefinition[] summaries, Expression<Func<T, bool>> filter) {
            using(var context = createContext()) {
                var query = getDbSet(context)
                    .Select(getEntityExpression)
                    .Where(filter);
                return query.GetSummaries(summaries);
            }
        }

        ValueAndCount[] IDataProvider<T>.GetDistinctValues(string propertyName, Expression<Func<T, bool>> filter) {
            using(var context = createContext()) {
                var query = getDbSet(context)
                    .Select(getEntityExpression)
                    .Where(filter);
                return query.DistinctWithCounts(propertyName);
            }
        }
    }
}
