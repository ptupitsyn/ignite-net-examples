using System;
using System.Collections.Generic;
using System.IO;

namespace IgniteEFCacheStore
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //Ignition.StartFromApplicationConfiguration();

            InitializeDb();
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
