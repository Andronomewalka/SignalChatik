using Microsoft.AspNetCore.Mvc;

namespace SignalChatik.Controllers
{
    public class ChatikContextController : ControllerBase
    {
        protected readonly ChatikContext context;

        public ChatikContextController(ChatikContext context)
        {
            this.context = context;
        }
    }
}
