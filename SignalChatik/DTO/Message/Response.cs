using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.DTO.Message
{
    public class MessageResponseDTO
    {
        public MessageDTO Message { get; set; }
    }

    public class GetMessagesResponseDTO
    {
        public IEnumerable<MessageDTO> Messages { get; set; }
    }
}
