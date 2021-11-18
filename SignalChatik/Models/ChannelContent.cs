using System;
using System.Collections.Generic;

#nullable disable

namespace SignalChatik.Models
{
    public partial class ChannelContent
    {
        public ChannelContent()
        {
            Rooms = new HashSet<Room>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
