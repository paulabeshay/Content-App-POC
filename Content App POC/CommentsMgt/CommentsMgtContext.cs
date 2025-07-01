using Microsoft.EntityFrameworkCore;

namespace Content_App_POC.CommentsMgt
{
    public class CommentsMgtContext : DbContext
    {
        public CommentsMgtContext(DbContextOptions<CommentsMgtContext> options)
            : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<CommentStatus> CommentStatuses { get; set; } = null!;
    }
} 