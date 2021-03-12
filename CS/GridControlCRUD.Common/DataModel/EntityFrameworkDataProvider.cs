using DevExpress.Xpf.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DevExpress.CRUD.DataModel.EntityFramework {
    public class EntityFrameworkDataProvider<TContext, TEntity, T> : IDataProvider<T>
        where TEntity : class
        where T : class
        where TContext : DbContext {

        protected readonly Func<TContext> createContext;
        protected readonly Func<TContext, DbSet<TEntity>> getDbSet;
        protected readonly Expression<Func<TEntity, T>> getEntityExpression;
        readonly string keyProperty;

        readonly Func<T, object> getKey;
        readonly Func<TEntity, object> getEntityKey;
        readonly Action<T, object> setKey;
        readonly Action<T, TEntity> applyProperties;

        public EntityFrameworkDataProvider(Func<TContext> createContext, Func<TContext, DbSet<TEntity>> getDbSet, Expression<Func<TEntity, T>> getEnityExpression, 
            string keyProperty = null, Func<T, object> getKey = null, Func<TEntity, object> getEntityKey = null, Action<T, object> setKey = null, Action<T, TEntity> applyProperties = null) {
            this.createContext = createContext;
            this.getDbSet = getDbSet;
            this.getEntityExpression = getEnityExpression;

            this.keyProperty = keyProperty;
            this.getKey = getKey ?? (_ => throw new NotSupportedException());
            this.getEntityKey = getEntityKey ?? (_ => throw new NotSupportedException());
            this.setKey = setKey ?? ((_, __) => throw new NotSupportedException());
            this.applyProperties = applyProperties ?? ((_, __) => throw new NotSupportedException());
        }

        IList<T> IDataProvider<T>.Read() {
            using(var context = createContext()) {
                var query = getDbSet(context)
                    .Select(getEntityExpression);
                return query.ToList();
            }
        }

        void IDataProvider<T>.Delete(T obj) {
            using(var context = createContext()) {
                var entity = getDbSet(context).Find(getKey(obj));
                if(entity == null) {
                    throw new NotImplementedException("The modified row no longer exists in the database. Handle this case according to your requirements.");
                }
                getDbSet(context).Remove(entity);
                SaveChanges(context);
            }
        }

        void IDataProvider<T>.Create(T obj) {
            using(var context = createContext()) {
                var entity = getDbSet(context).Create();
                getDbSet(context).Add(entity);
                applyProperties(obj, entity);
                SaveChanges(context);
                setKey(obj, getEntityKey(entity));
            }
        }

        void IDataProvider<T>.Update(T obj) {
            using(var context = createContext()) {
                var entity = getDbSet(context).Find(getKey(obj));
                if(entity == null) {
                    throw new NotImplementedException("The modified row does not exist in a database anymore. Handle this case according to your requirements.");
                }
                applyProperties(obj, entity);
                SaveChanges(context);
            }
        }

        TResult IDataProvider<T>.GetQueryableResult<TResult>(Func<IQueryable<T>, TResult> getResult) {
            using(var context = createContext()) {
                var queryable = getDbSet(context).Select(getEntityExpression);
                return getResult(queryable);
            }
        }

        string IDataProvider<T>.KeyProperty => keyProperty;

        static void SaveChanges(TContext context) {
            try {
                context.SaveChanges();
            } catch(Exception e) {
                throw ConvertException(e);
            }
        }

        static DbException ConvertException(Exception e) {
            var entityValidationException = e as DbEntityValidationException;
            if(entityValidationException != null) {
                var stringBuilder = new StringBuilder();
                foreach(var validationResult in entityValidationException.EntityValidationErrors) {
                    foreach(var error in validationResult.ValidationErrors) {
                        if(stringBuilder.Length > 0)
                            stringBuilder.AppendLine();
                        stringBuilder.Append(error.PropertyName + ": " + error.ErrorMessage);
                    }
                }
                return new DbException(stringBuilder.ToString(), entityValidationException);
            }
            return new DbException("An error has occurred while updating the database.", entityValidationException);
        }
    }
    public class DbException : Exception {
        public DbException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }
}
