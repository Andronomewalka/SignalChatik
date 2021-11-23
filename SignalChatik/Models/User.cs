using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalChatik.Models
{
    public class User : Channel
    {
        [Required]
        [ForeignKey("AuthUserId")]
        public AuthUser Auth { get; set; }
        public Guid AuthUserId { get; set; }
    }
}
