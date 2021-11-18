using System;
using System.Collections.Generic;

#nullable disable

namespace SignalChatik.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }

        public virtual User Receiver { get; set; }
        public virtual User Sender { get; set; }
    }
}
