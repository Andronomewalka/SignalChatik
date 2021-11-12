using Microsoft.EntityFrameworkCore;
using SignalChatik.Models;
using System;
using System.Configuration;

namespace SignalChatik
{
    public class ChatikContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> Roles { get; set; }
        public DbSet<UserRefreshToken> RefreshTokens { get; set; }

        public ChatikContext(DbContextOptions<ChatikContext> options) : base(options)
        {

        }
    }
}
