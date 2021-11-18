using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SignalChatik.Models;

#nullable disable

namespace SignalChatik
{
    public partial class ChatikContext : DbContext
    {
        public ChatikContext(DbContextOptions<ChatikContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AuthRole> AuthRoles { get; set; }
        public virtual DbSet<AuthUser> AuthUsers { get; set; }
        public virtual DbSet<AuthUserRefreshToken> AuthUserRefreshTokens { get; set; }
        public virtual DbSet<ChannelContent> ChannelContents { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomUser> RoomUsers { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserUser> UserUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<AuthRole>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<AuthUser>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Hash)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.AuthRole)
                    .WithMany(p => p.AuthUsers)
                    .HasForeignKey(d => d.AuthRoleId)
                    .HasConstraintName("FK__AuthUser__AuthRo__0A688BB1");
            });

            modelBuilder.Entity<AuthUserRefreshToken>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.RefreshToken).IsRequired();

                entity.HasOne(d => d.AuthUser)
                    .WithMany(p => p.AuthUserRefreshTokens)
                    .HasForeignKey(d => d.AuthUserId)
                    .HasConstraintName("FK__AuthUserR__AuthU__0B5CAFEA");
            });

            modelBuilder.Entity<ChannelContent>(entity =>
            {
                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Data).IsRequired();

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.MessageReceivers)
                    .HasForeignKey(d => d.ReceiverId)
                    .HasConstraintName("FK__Message__Receive__1209AD79");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.MessageSenders)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK__Message__SenderI__11158940");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Content)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.ContentId)
                    .HasConstraintName("FK__Room__ContentId__7EF6D905");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK__Room__OwnerId__7FEAFD3E");
            });

            modelBuilder.Entity<RoomUser>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomUsers)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__RoomUser__RoomId__02C769E9");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RoomUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__RoomUser__UserId__03BB8E22");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Auth)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.AuthId)
                    .HasConstraintName("FK__Users__AuthId__13F1F5EB");

                entity.HasOne(d => d.Content)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.ContentId)
                    .HasConstraintName("FK__User__ContentId__7B264821");
            });

            modelBuilder.Entity<UserUser>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.ConnectedUser)
                    .WithMany()
                    .HasForeignKey(d => d.ConnectedUserId)
                    .HasConstraintName("FK__UserUsers__Conne__16CE6296");

                entity.HasOne(d => d.ForUser)
                    .WithMany()
                    .HasForeignKey(d => d.ForUserId)
                    .HasConstraintName("FK__UserUsers__ForUs__15DA3E5D");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
