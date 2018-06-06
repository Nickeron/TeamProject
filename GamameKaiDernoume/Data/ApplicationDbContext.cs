using System;
using System.Collections.Generic;
using System.Text;
using TeamProject.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TeamProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Interest> Interests { get; set; }

        public DbSet<Friend> Friends { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(m => m.SentMessages);

            modelBuilder.Entity<User>()
                .HasMany(m => m.ReceivedMessages);

            modelBuilder.Entity<User>()
                .HasMany(m => m.Posts);

            modelBuilder.Entity<Message>()
                .HasOne(r => r.Receiver)
                .WithMany(m => m.ReceivedMessages);
                
            modelBuilder.Entity<Message>()
                .HasOne(r => r.Sender)
                .WithMany(m => m.SentMessages);

            modelBuilder.Entity<Friend>()
                .HasOne(r => r.Sender);

            modelBuilder.Entity<Friend>()
                .HasOne(r => r.Receiver)
                .WithMany(m => m.Friends)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostInterest>()
                .HasKey(pk => new { pk.PostId, pk.InterestId });

            modelBuilder.Entity<PostInterest>()
                .HasOne(p => p.Post)
                .WithMany(p => p.PostInterests)
                .HasForeignKey(p => p.PostId);

            modelBuilder.Entity<PostInterest>()
                .HasOne(p => p.Interest)
                .WithMany(p => p.PostInterests)
                .HasForeignKey(p => p.InterestId);

            base.OnModelCreating(modelBuilder);
        }


    }
}
