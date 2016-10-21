using System;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Apache.Ignite.Core.Cache.Store;

namespace IgniteEFCacheStore
{
    /// <summary>
    /// Generic EF cache store.
    /// </summary>
    public class EntityFrameworkCacheStore<TEntity, TContext> : ICacheStore 
        where TEntity : class, new() where TContext : DbContext
    {
        private readonly Func<TContext> _getContext;

        private readonly Func<TContext, IDbSet<TEntity>> _getDbSet;

        private readonly Func<TEntity, object> _getKey;

        private readonly Action<TEntity, object> _setKey;

        public EntityFrameworkCacheStore(Func<TContext> getContext, Func<TContext, IDbSet<TEntity>> getDbSet, 
            Func<TEntity, object> getKey, Action<TEntity, object> setKey)
        {
            if (getContext == null)
                throw new ArgumentNullException(nameof(getContext));

            if (getDbSet == null)
                throw new ArgumentNullException(nameof(getDbSet));

            if (getKey == null)
                throw new ArgumentNullException(nameof(getKey));

            if (setKey == null)
                throw new ArgumentNullException(nameof(setKey));

            _getContext = getContext;
            _getDbSet = getDbSet;
            _getKey = getKey;
            _setKey = setKey;
        }

        public void LoadCache(Action<object, object> act, params object[] args)
        {
            Console.WriteLine("{0}.LoadCache() called.", GetType().Name);

            // Load everything from DB to Ignite
            using (var ctx = _getContext())
            {
                foreach (var entity in _getDbSet(ctx))
                {
                    act(_getKey(entity), entity);
                }
            }
        }

        public object Load(object key)
        {
            Console.WriteLine("{0}.Load({1}) called.", GetType().Name, key);

            using (var ctx = _getContext())
            {
                return _getDbSet(ctx).Find(key);
            }
        }

        public IDictionary LoadAll(ICollection keys)
        {
            Console.WriteLine("{0}.LoadAll({1}) called.", GetType().Name, keys);

            // TODO: Load in one SQL query.
            return keys.OfType<object>().ToDictionary(x => x, Load);
        }

        public void Write(object key, object val)
        {
            Console.WriteLine("{0}.Write({1}, {2}) called.", GetType().Name, key, val);

            using (var ctx = _getContext())
            {
                _getDbSet(ctx).AddOrUpdate((TEntity) val);

                ctx.SaveChanges();
            }
        }

        public void WriteAll(IDictionary entries)
        {
            Console.WriteLine("{0}.WriteAll({1}) called.", GetType().Name, entries);

            using (var ctx = _getContext())
            {
                foreach (var entity in entries.Values.OfType<TEntity>())
                {
                    _getDbSet(ctx).AddOrUpdate(entity);

                    ctx.SaveChanges();
                }
            }
        }

        public void Delete(object key)
        {
            Console.WriteLine("{0}.Delete({1}) called.", GetType().Name, key);

            using (var ctx = _getContext())
            {
                var entity = new TEntity();
                _setKey(entity, key);

                _getDbSet(ctx).Remove(entity);

                ctx.SaveChanges();
            }
        }

        public void DeleteAll(ICollection keys)
        {
            Console.WriteLine("{0}.DeleteAll({1}) called.", GetType().Name, keys);

            using (var ctx = _getContext())
            {
                var dbSet = _getDbSet(ctx);

                foreach (var key in keys)
                {
                    var entity = new TEntity();
                    _setKey(entity, key);

                    dbSet.Remove(entity);
                }

                ctx.SaveChanges();
            }
        }

        public void SessionEnd(bool commit)
        {
            // No-op.
        }
    }
}
