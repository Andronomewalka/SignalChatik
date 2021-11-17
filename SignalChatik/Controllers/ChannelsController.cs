using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignalChatik.Helpers;
using SignalChatik.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ChannelsController : ChatikContextController
    {
        public ChannelsController(ChatikContext context) : base(context)
        {

        }

        [HttpGet]
        [Authorize(Roles = "User")]
        [Route("")]
        public async Task<IActionResult> GetChannels()
        {
            object accessToken = Request.Headers["Authorization"][0];

            JwtSecurityToken decodedAccessToken =
    JwtHelper.ReadToken(accessToken,
        jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters);
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> Test()
        {
            Channel user1Channel = new Channel()
            {
                Name = "some user",
                ChannelTypeId = ChannelTypeId.User
            };

            Channel user2Channel = new Channel()
            {
                Name = "another user",
                ChannelTypeId = ChannelTypeId.User,
                ConnectedChannels = new List<Channel>()
                {
                    user1Channel
                }
            };

            Channel roomChannel = new Channel()
            {
                Name = "some room",
                ChannelTypeId = ChannelTypeId.Room,
                ConnectedChannels = new List<Channel>()
                {
                    user1Channel, user2Channel
                }
            };

            Message fromUser1ToUser2 = new Message()
            {
                SenderChannel = user1Channel,
                ReceiverChannel = user2Channel,
                Data = "hello"
            };

            await context.Channels.AddRangeAsync(user1Channel, user2Channel, roomChannel);
            await context.Messages.AddAsync(fromUser1ToUser2);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
