using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;

namespace IgniteEFCacheStore
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            InitializeDb();

            using (var ignite = Ignition.StartFromApplicationConfiguration())
            {
                var blogs = ignite.GetOrCreateCache<int, Blog>(new CacheConfiguration
                {
                    Name = "blogs",
                    CacheStoreFactory = new BlogCacheStoreFactory(),
                    ReadThrough = true,
                    WriteThrough = true,
                    KeepBinaryInStore = false   // Store works with concrete classes.
                });

                var posts = ignite.GetOrCreateCache<int, Post>(new CacheConfiguration
                {
                    Name = "posts",
                    CacheStoreFactory = new PostCacheStoreFactory(),
                    ReadThrough = true,
                    WriteThrough = true,
                    KeepBinaryInStore = false   // Store works with concrete classes.
                });

                Console.WriteLine("\n>>> Example started\n\n");

                // Load all posts, but do not load blogs.
                Console.WriteLine("Calling ICache.LoadCache...");
                posts.LoadCache(null);

                // Show all posts with their blogs.
                DisplayData(posts, blogs);

                // Add new data to cache.
                Console.WriteLine("Adding new post to existing blog..");

                var postId = posts.Max(x => x.Key) + 1;  // Generate new id.

                posts[1] = new Post
                {
                    BlogId = blogs.Min(x => x.Key), // Existing blog
                    PostId = postId,
                    Title = "New Post From Ignite"
                };

                // Show all posts with their blogs.
                DisplayData(posts, blogs);

                // Remove newly added post.
                Console.WriteLine("Removing post with id {0}...", postId);
                posts.Remove(postId);

                Console.WriteLine("\n>>> Example finished.\n");
            }
        }

        private static void DisplayData(ICache<int, Post> posts, ICache<int, Blog> blogs)
        {
            Console.WriteLine("\n>>> List of all posts:");

            foreach (var post in posts) // Cache iterator does not go to store.
            {
                Console.WriteLine("Retrieving blog with id {0}...", post.Value.BlogId);
                var blog = blogs[post.Value.BlogId]; // Retrieving by key goes to store.

                Console.WriteLine(">>> Post '{0}' in blog '{1}'", post.Value.Title, blog.Name);
            }

            Console.WriteLine(">>> End list.\n");
        }

        private static void InitializeDb()
        {
            using (var ctx = new BloggingContext())
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                var dataSource = Path.GetFullPath(ctx.Database.Connection.DataSource);

                if (ctx.Database.CreateIfNotExists())
                {
                    var blog = new Blog
                    {
                        Name = "Ignite Blog",
                        Posts = new List<Post>
                        {
                            new Post
                            {
                                Title = "Getting Started With Ignite.NET",
                                Content = "Refer to https://ptupitsyn.github.io/Getting-Started-With-Apache-Ignite-Net/"
                            }
                        }
                    };

                    ctx.Blogs.Add(blog);

                    Console.WriteLine("Database created at {0} with {1} entities.", dataSource, ctx.SaveChanges());
                }
                else
                {
                    Console.WriteLine("Database already exists at {0}.", dataSource);
                }
            }
        }
    }
}
