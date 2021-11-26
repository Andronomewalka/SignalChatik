using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.DTO.Message
{
    public enum MessageType
    {
        Sender, Receiver
    }

    public class MessageDTO
    {
        public int Id { get; set; }
        public string User { get; set; }
        public int ReceiverId { get; set; }
        public DateTime DateUtc { get; set; }
        public MessageType Type { get; set; }
        public string Text { get; set; }
    }
}
