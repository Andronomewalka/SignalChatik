using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalChatik.DTO;
using SignalChatik.Helpers;
using SignalChatik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalChatik.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class MessagesController : ChatikContextController
    {
        private Guid UserId => Guid.Parse(User.Claims.Single(cur => cur.Type == ClaimTypes.NameIdentifier).Value);

        public MessagesController(ChatikContext context) : base(context)
        {
        }


        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult Get([FromQuery]int channelId)
        {
            try
            {
                if (channelId < 0)
                    return JsonResponse.CreateBad(400, $"ChannelId is invalid");

                User associatedUser = context.Users
                    .Include(cur => cur.Channel)
                    .FirstOrDefault(cur => cur.Auth.Guid.ToString() == UserId.ToString());

                if (associatedUser == null)
                    return JsonResponse.CreateBad(401, $"No auth for user guid found");

                Channel requestedChannel = context.Channels.FirstOrDefault(cur => cur.Id == channelId);
                if (requestedChannel == null)
                    return JsonResponse.CreateBad(404, $"Channel doesn't exist");

                //var sss = context.Messages
                //    .Include(cur => cur.Sender)
                //    .Include(cur => cur.Receiver);

                //var some = sss
                //    .Where(cur => (cur.Receiver == requestedChannel && cur.Sender == associatedUser.Channel));

                //var any = sss
                //    .Where(cur => (cur.Receiver == associatedUser.Channel && cur.Sender == requestedChannel));

                var messages = context.Messages
                    .Include(cur => cur.Sender)
                    .Include(cur => cur.Receiver)
                    .Where(cur => (cur.Receiver == requestedChannel && cur.Sender == associatedUser.Channel) ||
                                   (cur.Receiver == associatedUser.Channel && cur.Sender == requestedChannel))
                    .ToList()
                    .Select(cur => new MessageDTO()
                    {
                        Id = cur.Id,
                        User = cur.Receiver == requestedChannel ? cur.Sender.Name : cur.Receiver.Name,
                        ReceiverId = cur.ReceiverId,
                        DateUtc = DateTime.UtcNow,
                        Type = cur.Receiver == requestedChannel ? MessageType.Sender : MessageType.Receiver,
                        Text = cur.Data
                    });

                return JsonResponse.CreateGood(new GetMessagesResponseDTO()
                {
                    Messages = messages
                });
            }
            catch (Exception e)
            {
                return JsonResponse.CreateBad(HttpStatusCode.InternalServerError, "Server fucked up");
            }
        }
    }
 }
