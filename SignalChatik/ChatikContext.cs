using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SignalChatik.Models;
using SignalChatik.Models.Configs;

namespace SignalChatik
{
    public partial class ChatikContext : DbContext
    {
        public DbSet<AuthUserRole> AuthUserRoles { get; set; }
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<AuthUserRefreshToken> AuthUserRefreshTokens { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ConnectedChannel> ConnectedChannels { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ChatikContext(DbContextOptions<ChatikContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AuthUserConfig());
            modelBuilder.ApplyConfiguration(new ChannelConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new RoomConfig());
        }
    }
}
