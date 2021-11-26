using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalChatik.DTO;
using SignalChatik.DTO.Channel;
using SignalChatik.DTO.Room;
using SignalChatik.DTO.User;
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
    public class RoomsController : ChatikContextController
    {
        private Guid UserId => Guid.Parse(User.Claims.Single(cur => cur.Type == ClaimTypes.NameIdentifier).Value);

        public RoomsController(ChatikContext context) : base(context)
        {
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<User>> CreateRoom([FromBody] RoomRequestDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return JsonResponse.CreateBad(400, "Room is empty");

                User associatedUser = context.Users
                    .Include(cur => cur.Channel)
                    .FirstOrDefault(cur => cur.Auth.Guid.ToString() == UserId.ToString());

                if (associatedUser == null)
                    return JsonResponse.CreateBad(401, "No auth for user guid found");

                var roomAlreadyExisted = await context.Rooms
                    .Include(cur => cur.Channel)
                    .AnyAsync(cur => cur.Channel.Name == request.Name);

                if (roomAlreadyExisted)
                    return JsonResponse.CreateBad(409, "Name is taken");

                Room createdRoom = new Room()
                {
                    Channel = new Channel()
                    {
                        Name = request.Name,
                        Description = request.Description,
                        ChannelTypeId = ChannelType.Room,
                    },
                    Owner = associatedUser
                };
                await context.Rooms.AddAsync(createdRoom);

                await context.ConnectedChannels.AddAsync(new ConnectedChannel()
                {
                    For = associatedUser.Channel,
                    Connected = createdRoom.Channel
                });

                await context.SaveChangesAsync();

                UserDTO ownerDTO = new UserDTO()
                {
                    Id = associatedUser.Channel.Id,
                    Name = associatedUser.Channel.Name,
                    Description = associatedUser.Channel.Description
                };

                return JsonResponse.CreateGood(new RoomResponseDTO()
                {
                    Room = new RoomDTO()
                    {
                        Id = createdRoom.Channel.Id,
                        Name = createdRoom.Channel.Name,
                        Description = createdRoom.Channel.Description,
                        Owner = ownerDTO,
                        Members = new List<UserDTO>()
                        {
                            ownerDTO
                        }
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
