using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.DTO.Message
{
    public class MessageRequestDTO
    {
        public int ChannelId { get; set; } = -1;
        public string Message { get; set; } = "";
    }
}
