using Microsoft.EntityFrameworkCore;
using SignalChatik.Models;
using System;
using System.Configuration;
using System.Linq;

namespace SignalChatik
{
    public class ChatikContext : DbContext
    {
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<AuthUserRole> AuthRoles { get; set; }
        public DbSet<AuthUserRefreshToken> AuthRefreshTokens { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChannelType> ChannelTypes { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ChatikContext(DbContextOptions<ChatikContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BuildAuthRoleEnums(modelBuilder);
            BuildChannelTypeEnums(modelBuilder);

            modelBuilder
                .Entity<Channel>()
                .HasMany(p => p.ForChannels)
                .WithMany(p => p.ConnectedChannels)
                .UsingEntity(j => j.ToTable("ChannelChannels"));
        }

        private void BuildChannelTypeEnums(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Channel>()
                .Property(e => e.ChannelTypeId)
                .HasConversion<int>();

            modelBuilder
                .Entity<ChannelType>()
                .Property(e => e.Id)
                .HasConversion<int>();

            modelBuilder
                .Entity<ChannelType>().HasData(
                    Enum.GetValues(typeof(ChannelTypeId))
                        .Cast<ChannelTypeId>()
                        .Select(e => new ChannelType()
                        {
                            Id = e,
                            Name = e.ToString()
                        })
                );
        }

        private void BuildAuthRoleEnums(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<AuthUser>()
                .Property(e => e.AuthUserRoleId)
                .HasConversion<int>();

            modelBuilder
                .Entity<AuthUserRole>()
                .Property(e => e.Id)
                .HasConversion<int>();

            modelBuilder
                .Entity<AuthUserRole>().HasData(
                    Enum.GetValues(typeof(AuthUserRoleId))
                        .Cast<AuthUserRoleId>()
                        .Select(e => new AuthUserRole()
                        {
                            Id = e,
                            Name = e.ToString()
                        })
                );
        }
    }
}
