using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalChatik.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public Channel Channel { get; set; }
        public int ChannelId { get; set; }

        [Required]
        [ForeignKey("AuthUserId")]
        public AuthUser Auth { get; set; }
        public int AuthUserId { get; set; }

        public ICollection<Room> OwnedRooms { get; set; }
    }
}
