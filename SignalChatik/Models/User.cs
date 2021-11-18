using System;
using System.Collections.Generic;

#nullable disable

namespace SignalChatik.Models
{
    public partial class User
    {
        public User()
        {
            MessageReceivers = new HashSet<Message>();
            MessageSenders = new HashSet<Message>();
            RoomUsers = new HashSet<RoomUser>();
            Rooms = new HashSet<Room>();
        }

        public int Id { get; set; }
        public int? ContentId { get; set; }
        public int? AuthId { get; set; }

        public virtual AuthUser Auth { get; set; }
        public virtual ChannelContent Content { get; set; }
        public virtual ICollection<Message> MessageReceivers { get; set; }
        public virtual ICollection<Message> MessageSenders { get; set; }
        public virtual ICollection<RoomUser> RoomUsers { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
