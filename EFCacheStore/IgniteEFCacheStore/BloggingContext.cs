using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace IgniteEFCacheStore
{
    public class BloggingContext : DbContext
    {
        public BloggingContext() : base("DataSource=blogs.db")
        {
            // No-op.
        }

        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
    }

    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BlogId { get; set; }
        public string Name { get; set; }

        // Navigation property
        public virtual List<Post> Posts { get; set; }
    }

    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int BlogId { get; set; }

        // Navigation property
        public virtual Blog Blog { get; set; }
    }
}
