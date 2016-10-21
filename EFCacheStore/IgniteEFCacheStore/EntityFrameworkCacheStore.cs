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
        where TEntity : class where TContext : DbContext
    {
        private readonly Func<TContext> _getContext;

        private readonly Func<TContext, IDbSet<TEntity>> _getDbSet;

        private readonly Func<TEntity, object> _getKey;

        public EntityFrameworkCacheStore(Func<TContext> getContext, Func<TContext, IDbSet<TEntity>> getDbSet, 
            Func<TEntity, object> getKey)
        {
            _getContext = getContext;
            _getDbSet = getDbSet;
            _getKey = getKey;
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
            using (var ctx = _getContext())
            {
                return keys.Cast<int>().ToDictionary(key => key, key => _getDbSet(ctx).Find(key));
            }
        }

        public void Write(object key, object val)
        {
            Console.WriteLine("{0}.Write({1}, {2}) called.", GetType().Name, key, val);

            using (var ctx = _getContext())
            {
                _getDbSet(ctx).AddOrUpdate((TEntity)val);

                ctx.SaveChanges();
            }
        }

        public void WriteAll(IDictionary entries)
        {
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
                var entity = _getDbSet(ctx).Find(key);

                if (entity != null)
                {
                    _getDbSet(ctx).Remove(entity);

                    ctx.SaveChanges();
                }
            }
        }

        public void DeleteAll(ICollection keys)
        {
            foreach (var key in keys)
            {
                Delete(key);
            }
        }

        public void SessionEnd(bool commit)
        {
            // No-op.
        }
    }
}
