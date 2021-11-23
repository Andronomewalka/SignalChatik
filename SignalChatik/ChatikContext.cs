using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SignalChatik.Models;

namespace SignalChatik
{
    public partial class ChatikContext : DbContext
    {
        public DbSet<AuthUserRole> AuthUserRoles { get; set; }
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<AuthUserRefreshToken> AuthUserRefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<ChannelContent> ChannelContents { get; set; }


        public ChatikContext(DbContextOptions<ChatikContext> options)
            : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clustered>()
                .HasIndex(e => e.ClusterID)
                .IsUnique()
                .IsClustered();

            modelBuilder.Entity<Clustered>()
                .HasKey("Id")
                .IsClustered(false);

            modelBuilder.Ignore<Clustered>();

            modelBuilder.Entity<AuthUser>().ToTable("AuthUsers");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Room>().ToTable("Rooms");

            modelBuilder.Ignore<Channel>();

            //AllThatInherits<Clustered>(modelBuilder, (entityType) =>
            //{
            //    modelBuilder.Entity(entityType.ClrType)
            //      .HasIndex(nameof(Clustered.ClusterID))
            //      .IsUnique()
            //      .IsClustered();

            //    modelBuilder.Entity(entityType.ClrType)
            //        .HasIndex(nameof(Clustered.ClusterID))
            //        .IsUnique()
            //        .IsClustered();
            //});

            //modelBuilder.Entity<AuthUser>()
            //    .HasOne(cur => cur.User)
            //    .WithOne(cur => cur.Auth)
            //    .HasForeignKey<User>(cur => cur.Id);

            //modelBuilder.Entity<Channel>().HasBaseType<Clustered>();

            //AllThatInherits<Channel>(modelBuilder, (entityType) =>
            //{
            //    modelBuilder.Entity(entityType.ClrType)
            //        .HasOne(nameof(Channel.Content))
            //        .WithOne(nameof(ChannelContent.Channel))
            //        .HasForeignKey(nameof(ChannelContent.ChannelId));
            //});

            //modelBuilder.Ignore<Channel>();

            //modelBuilder.Entity<User>().HasBaseType<Channel>();
            //modelBuilder.Entity<Room>().HasBaseType<Channel>();
        }
    }
}
