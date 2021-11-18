using System;
using System.Collections.Generic;

#nullable disable

namespace SignalChatik.Models
{
    public partial class RoomUser
    {
        public int Id { get; set; }
        public int? RoomId { get; set; }
        public int? UserId { get; set; }

        public virtual Room Room { get; set; }
        public virtual User User { get; set; }
    }
}
