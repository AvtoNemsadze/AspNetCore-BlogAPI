using BlogAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;


namespace BlogAPI.Core.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
    }
}
