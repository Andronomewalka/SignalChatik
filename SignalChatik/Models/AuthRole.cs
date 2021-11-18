using System;
using System.Collections.Generic;

#nullable disable

namespace SignalChatik.Models
{
    public partial class AuthRole
    {
        public AuthRole()
        {
            AuthUsers = new HashSet<AuthUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AuthUser> AuthUsers { get; set; }
    }
}
