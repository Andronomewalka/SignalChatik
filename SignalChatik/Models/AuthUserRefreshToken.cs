using System;
using System.Collections.Generic;

#nullable disable

namespace SignalChatik.Models
{
    public partial class AuthUserRefreshToken
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        public int? AuthUserId { get; set; }

        public virtual AuthUser AuthUser { get; set; }
    }
}
