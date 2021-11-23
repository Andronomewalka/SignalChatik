using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SignalChatik.Models
{
    public partial class AuthUser : Clustered
    {
        [Required]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Hash cannot exceed 100 characters")]
        public string Hash { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Salt cannot exceed 100 characters")]
        public string Salt { get; set; }


        [Required]
        public ICollection<AuthUserRefreshToken> RefreshTokens { get; set; }

        [Required]
        public ICollection<AuthUserRole> Roles { get; set; }

        public User User { get; set; }
    }
}
