using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.Helpers
{
    public enum ChatikHubMethods
    {
        SendMessageFailed,
        SendMessageSuccess,
        ReceiveMessage,
        ChannelConnected
    }
}
