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

        public EntityFrameworkDataProvider(Func<TContext> createContext, Func<TContext, DbSet<TEntity>> getDbSet, Expression<Func<TEntity, T>> getEnityExpression) {
            this.createContext = createContext;
            this.getDbSet = getDbSet;
            this.getEntityExpression = getEnityExpression;
        }

        IList<T> IDataProvider<T>.Read() {
            using(var context = createContext()) {
                var query = getDbSet(context)
                    .Select(getEntityExpression);
                return query.ToList();
            }
        }
    }
}
