using System;
using System.Collections.Generic;

#nullable disable

namespace SignalChatik.Models
{
    public partial class Room
    {
        public Room()
        {
            RoomUsers = new HashSet<RoomUser>();
        }

        public int Id { get; set; }
        public int? ContentId { get; set; }
        public int? OwnerId { get; set; }

        public virtual ChannelContent Content { get; set; }
        public virtual User Owner { get; set; }
        public virtual ICollection<RoomUser> RoomUsers { get; set; }
    }
}
