using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moiniorell.Domain.Models;
using System.Reflection.Emit;

namespace Moiniorell.Persistence.DAL
{
    //public class AppDbContext : IdentityDbContext<AppUser,IdentityRole<int>,int>
    
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions opt) : base(opt) { }

        public DbSet<Follow> Follows { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Follow>()                                            //  1.
                .HasKey(k => new { k.FollowerId, k.FolloweeId });

            builder.Entity<Follow>()                                            //  2.
                .HasOne(u => u.Followee)
                .WithMany(u => u.Followers)
                .HasForeignKey(u => u.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Follow>()                                            //  3.
                .HasOne(u => u.Follower)
                .WithMany(u => u.Followees)
                .HasForeignKey(u => u.FolloweeId)
            .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Post>(entity =>
            {
                entity.HasOne<AppUser>(a => a.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(id => id.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany<Comment>(c => c.Comments)
                .WithOne(p => p.CommentedPost)
                .HasForeignKey(i => i.CommentedPostId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<Comment>(entity =>
            {
                entity.HasOne(a => a.Author)
                .WithMany(c => c.Comments)
                .HasForeignKey(aId => aId.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(p => p.CommentedPost)
                .WithMany(c => c.Comments)
                .HasForeignKey(pId => pId.CommentedPostId);

            });


            builder.Entity<AppUser>(entity =>
            {
                entity.HasMany<Post>(p => p.Posts)
                .WithOne(a => a.Author)
                .HasForeignKey(pId => pId.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany<Comment>(c => c.Comments)
                .WithOne(a => a.Author)
                .HasForeignKey(cId => cId.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }

}
