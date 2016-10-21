using System;
using System.Linq;
using Apache.Ignite.Core;

namespace IgniteEFCacheStore
{
    public static class Program
    {
        static void Main(string[] args)
        {
            //Ignition.StartFromApplicationConfiguration();

            using (var ctx = new BloggingContext())
            {
                ctx.Database.CreateIfNotExists();

                ctx.Blogs.Add(new Blog {Name = "MyBlog1"});

                ctx.SaveChanges();

                Console.WriteLine(ctx.Blogs.Count());
            }
        }
    }
}
