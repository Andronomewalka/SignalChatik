using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalChatik.DTO;
using SignalChatik.DTO.Channel;
using SignalChatik.DTO.Room;
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
        public async Task<IActionResult> Get()
        {
            try
            {
                User associatedUser = context.Users
                    .Include(cur => cur.Channel)
                    .AsNoTracking()
                    .FirstOrDefault(cur => cur.Auth.Guid.ToString() == UserId.ToString());

                if (associatedUser == null)
                    return JsonResponse.CreateBad(401, "No auth for user guid found");

                var allConnectedChannels = context.ConnectedChannels
                   .Include(cur => cur.For)
                   .Include(cur => cur.Connected)
                   .AsNoTrackingWithIdentityResolution();

                var connectedChannels = allConnectedChannels
                    .Where(cur => cur.For == associatedUser.Channel)
                    .Select(cur => new ChannelDTO()
                    {
                        Id = cur.Connected.Id,
                        Name = cur.Connected.Name,
                        Description = cur.Connected.Description,
                        Type = cur.Connected.ChannelTypeId,
                        LastInteractionTime = cur.LastInteractionTime
                    });

                var reverseConnectedChannels = allConnectedChannels
                    .Where(cur => cur.Connected == associatedUser.Channel && cur.IsConnectedBack)
                    .Select(cur => new ChannelDTO()
                    {
                        Id = cur.For.Id,
                        Name = cur.For.Name,
                        Description = cur.For.Description,
                        Type = cur.For.ChannelTypeId,
                        LastInteractionTime = cur.LastInteractionTime
                    });

                var resultConnectedChannels = connectedChannels
                    .Concat(reverseConnectedChannels)
                    .OrderByDescending(cur => cur.LastInteractionTime);

                return JsonResponse.CreateGood(new GetChannelsResponseDTO()
                {
                    Channels = resultConnectedChannels
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
        public async Task<ActionResult<User>> ConnectChannel([FromBody] string channelName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(channelName))
                    return JsonResponse.CreateBad(400, "Channel is empty");

                User associatedUser = context.Users
                .Include(cur => cur.Channel)
                .FirstOrDefault(cur => cur.Auth.Guid.ToString() == UserId.ToString());

                if (associatedUser == null)
                    return JsonResponse.CreateBad(401, "No auth for user guid found");

                Channel requestedChannel = context.Channels
                    .FirstOrDefault(cur => cur.Name == channelName);

                if (requestedChannel == null)
                    return JsonResponse.CreateBad(404, "Channel doesn't exist");

                if (await context.ConnectedChannels
                        .AsNoTrackingWithIdentityResolution()
                        .AnyAsync(cur => cur.For == associatedUser.Channel && cur.Connected == requestedChannel))
                    return JsonResponse.CreateBad(422, "Channel already connected");

                var reverseConnection = await context.ConnectedChannels
                    .FirstOrDefaultAsync(cur => cur.For == requestedChannel && cur.Connected == associatedUser.Channel);

                if (reverseConnection != null)
                {
                    if (reverseConnection.IsConnectedBack)
                        return JsonResponse.CreateBad(422, "Channel already connected");

                    reverseConnection.IsConnectedBack = true;
                }
                else
                {
                    await context.ConnectedChannels.AddAsync(new ConnectedChannel()
                    {
                        For = associatedUser.Channel,
                        Connected = requestedChannel,
                        IsConnectedBack = requestedChannel.ChannelTypeId == ChannelType.Room
                    });
                }

                await context.SaveChangesAsync();

                return JsonResponse.CreateGood(new ConnectChannelResponseDTO()
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
