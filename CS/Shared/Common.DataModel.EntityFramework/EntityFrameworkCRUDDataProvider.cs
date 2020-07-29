using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DevExpress.CRUD.DataModel.EntityFramework {
    public class EntityFrameworkCRUDDataProvider<TContext, TEntity, T, TKey> : EntityFrameworkDataProvider<TContext, TEntity, T>, ICRUDDataProvider<T>
        where TEntity : class
        where T : class
        where TContext : DbContext {

        readonly Func<T, TKey> getKey;
        readonly Func<TEntity, TKey> getEntityKey;
        readonly Action<T, TKey> setKey;
        readonly Action<T, TEntity> applyProperties;

        public EntityFrameworkCRUDDataProvider(
            Func<TContext> createContext, Func<TContext, DbSet<TEntity>> getDbSet, Expression<Func<TEntity, T>> getEnityExpression,
            Func<T, TKey> getKey, Func<TEntity, TKey> getEntityKey, Action<T, TKey> setKey, Action<T, TEntity> applyProperties)
            : base(createContext, getDbSet, getEnityExpression) {
            this.getKey = getKey;
            this.getEntityKey = getEntityKey;
            this.setKey = setKey;
            this.applyProperties = applyProperties;
        }

        void ICRUDDataProvider<T>.Delete(T obj) {
            using(var context = createContext()) {
                var entity = getDbSet(context).Find(getKey(obj));
                if(entity == null) {
                    throw new NotImplementedException("The modified row no longer exists in the database. Handle this case according to your requirements.");
                }
                getDbSet(context).Remove(entity);
                SaveChanges(context);
            }
        }

        void ICRUDDataProvider<T>.Create(T obj) {
            using(var context = createContext()) {
                var entity = getDbSet(context).Create();
                getDbSet(context).Add(entity);
                applyProperties(obj, entity);
                SaveChanges(context);
                setKey(obj, getEntityKey(entity));
            }
        }

        void ICRUDDataProvider<T>.Update(T obj) {
            using(var context = createContext()) {
                var entity = getDbSet(context).Find(getKey(obj));
                if(entity == null) {
                    throw new NotImplementedException("The modified row does not exist in a database anymore. Handle this case according to your requirements.");
                }
                applyProperties(obj, entity);
                SaveChanges(context);
            }
        }

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
