using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SignalChatik.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid Guid { get; set; }

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
        public List<UserRefreshToken> RefreshTokens { get; set; }

        [Required]
        public List<UserRole> Roles { get; set; }
    }
}
