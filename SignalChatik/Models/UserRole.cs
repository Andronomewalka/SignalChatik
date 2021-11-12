using System.ComponentModel.DataAnnotations;

namespace SignalChatik.Models
{
    public enum Role
    {
        User, Admin
    }

    public class UserRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EnumDataType(typeof(Role))]
        public Role Role { get; set; }

        public User User { get; set; }
    }
}
