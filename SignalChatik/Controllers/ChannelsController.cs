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
    public class ChannelsController : ChatikContextController
    {
        private Guid UserId => Guid.Parse(User.Claims.Single(cur => cur.Type == ClaimTypes.NameIdentifier).Value);

        public ChannelsController(ChatikContext context) : base(context)
        {
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult Get()
        {
            try
            {
                User associatedUser = context.Users
                    .Include(cur => cur.Channel)
                    .FirstOrDefault(cur => cur.Auth.Guid.ToString() == UserId.ToString());

                if (associatedUser == null)
                    return JsonResponse.CreateBad(401, $"No auth for user guid found");

                var connectedChannels = context.ConnectedChannels
                    .Include(cur => cur.For)
                    .Include(cur => cur.Connected)
                    .Where(cur => cur.For == associatedUser.Channel)
                    .Select(cur => new ChannelDTO()
                    {
                        Id = cur.Connected.Id,
                        Name = cur.Connected.Name,
                        Description = cur.Connected.Description,
                        Type = cur.Connected.ChannelTypeId
                    });

                return JsonResponse.CreateGood(new GetChannelsDTO()
                {
                    Channels = connectedChannels
                });
            }
            catch (Exception e)
            {
                return JsonResponse.CreateBad(HttpStatusCode.InternalServerError, "Server fucked up");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("connect")]
        public async Task<ActionResult<User>> Connect([FromBody] string channelName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(channelName))
                    return JsonResponse.CreateBad(400, $"Channel is empty");

                User associatedUser = context.Users
                .Include(cur => cur.Channel)
                .FirstOrDefault(cur => cur.Auth.Guid.ToString() == UserId.ToString());

                if (associatedUser == null)
                    return JsonResponse.CreateBad(401, $"No auth for user guid found");

                List<ConnectedChannel> connectedChannels = context.ConnectedChannels
                    .Where(cur => cur.For == associatedUser.Channel)
                    .ToList();

                Channel requestedChannel = context.Channels
                    .FirstOrDefault(cur => cur.Name == channelName);

                if (requestedChannel == null)
                    return JsonResponse.CreateBad(404, $"Channel doesn't exist");

                if (connectedChannels.Any(cur => cur.Connected == requestedChannel))
                    return JsonResponse.CreateBad(422, $"Channel already connected");

                await context.ConnectedChannels.AddAsync(new ConnectedChannel()
                {
                    For = associatedUser.Channel,
                    Connected = requestedChannel
                });
                await context.SaveChangesAsync();
                return JsonResponse.CreateGood(new ConnectChannelDTO()
                {
                    Channel = new ChannelDTO()
                    {
                        Id = requestedChannel.Id,
                        Name = requestedChannel.Name,
                        Description = requestedChannel.Description,
                        Type = requestedChannel.ChannelTypeId
                    }
                });
            }
            catch (Exception e)
            {
                return JsonResponse.CreateBad(HttpStatusCode.InternalServerError, "Server fucked up");
            }
        }
    }
}
