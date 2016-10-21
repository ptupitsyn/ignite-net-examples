using System;
using System.Collections;
using System.Data.Entity.Migrations;
using System.Linq;
using Apache.Ignite.Core.Cache.Store;

namespace IgniteEFCacheStore
{
    /// <summary>
    /// Ignite Cache Store for <see cref="Post"/> entities.
    /// </summary>
    public class PostCacheStore : ICacheStore
    {
        public void LoadCache(Action<object, object> act, params object[] args)
        {
            // Load everything from DB to Ignite
            using (var ctx = GetDbContext())
            {
                foreach (var post in ctx.Posts)
                {
                    act(post.PostId, post);
                }
            }
        }

        public object Load(object key)
        {
            using (var ctx = GetDbContext())
            {
                return ctx.Posts.Find(key);
            }
        }

        public IDictionary LoadAll(ICollection keys)
        {
            using (var ctx = GetDbContext())
            {
                return keys.Cast<int>().ToDictionary(key => key, key => ctx.Posts.Find(key));
            }
        }

        public void Write(object key, object val)
        {
            using (var ctx = GetDbContext())
            {
                ctx.Posts.AddOrUpdate((Post) val);

                ctx.SaveChanges();
            }
        }

        public void WriteAll(IDictionary entries)
        {
            using (var ctx = GetDbContext())
            {
                foreach (var post in entries.Values.OfType<Post>())
                {
                    ctx.Posts.AddOrUpdate(post);

                    ctx.SaveChanges();
                }
            }
        }

        public void Delete(object key)
        {
            using (var ctx = GetDbContext())
            {
                var post = ctx.Posts.Find(key);

                if (post != null)
                {
                    ctx.Posts.Remove(post);

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

        private static BloggingContext GetDbContext()
        {
            return new BloggingContext();
        }
    }
}
