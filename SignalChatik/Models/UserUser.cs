using System;
using System.Collections.Generic;

#nullable disable

namespace SignalChatik.Models
{
    public partial class UserUser
    {
        public int Id { get; set; }
        public int? ForUserId { get; set; }
        public int? ConnectedUserId { get; set; }

        public virtual User ConnectedUser { get; set; }
        public virtual User ForUser { get; set; }
    }
}
