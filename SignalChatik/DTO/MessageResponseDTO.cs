using System;
using System.Collections.Generic;

namespace SignalChatik.DTO
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

    public class MessageResponseDTO
    {
        public MessageDTO Message { get; set; }
    }

    public class GetMessagesResponseDTO
    {
        public IEnumerable<MessageDTO> Messages { get; set; }
    }
}
