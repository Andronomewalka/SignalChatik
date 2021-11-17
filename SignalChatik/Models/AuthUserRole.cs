using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SignalChatik.Models
{
    public enum AuthUserRoleId
    {
        User, Admin
    }

    public class AuthUserRole
    {
        [Key]
        public AuthUserRoleId Id { get; set; }

        public string Name { get; set; }

        public List<AuthUser> AuthUsers { get; set; }
    }
}
