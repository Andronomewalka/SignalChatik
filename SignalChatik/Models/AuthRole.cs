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
        public int Id { get; set; }

        [Required]
        public AuthUserRoleId RoleId { get; set; }


        [Required]
        public AuthUser AuthUser { get; set; }
    }
}
