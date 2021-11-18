using System;
using System.Collections.Generic;

#nullable disable

namespace SignalChatik.Models
{
    public partial class AuthUser
    {
        public AuthUser()
        {
            AuthUserRefreshTokens = new HashSet<AuthUserRefreshToken>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public int? AuthRoleId { get; set; }

        public virtual AuthRole AuthRole { get; set; }
        public virtual ICollection<AuthUserRefreshToken> AuthUserRefreshTokens { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
