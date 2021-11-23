//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using SignalChatik.DTO;
//using SignalChatik.Helpers;
//using SignalChatik.Models;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Net;
//using System.Security.Claims;
//using System.Threading.Channels;
//using System.Threading.Tasks;

//namespace SignalChatik.Controllers
//{
//    [Route("[controller]")]
//    [ApiController]
//    [Produces("application/json")]
//    public class ChannelsController : ChatikContextController
//    {
//        private readonly IOptionsMonitor<JwtBearerOptions> jwtOptions;
//        private Guid UserId => Guid.Parse(User.Claims.Single(cur => cur.Type == ClaimTypes.NameIdentifier).Value);

//        public ChannelsController(IOptionsMonitor<JwtBearerOptions> jwtOptions, ChatikContext context) : base(context)
//        {
//            this.jwtOptions = jwtOptions;
//        }

//        [HttpGet]
//        [Authorize(Roles = "User")]
//        [Route("")]
//        public IActionResult GetChannels()
//        {
//            try
//            {
//                User associatedUser = context.Users.FirstOrDefault(cur => cur.Auth.Guid.ToString() == UserId.ToString());
//                if (associatedUser == null)
//                {
//                    AuthUser authUser = context.AuthUsers.FirstOrDefault(cur => cur.Guid.ToString() == UserId.ToString());
//                    if (authUser == null)
//                        return JsonResponse.CreateBad(401, $"No auth for user guid found");

//                    associatedUser = new User()
//                    {
//                        Auth = authUser,
//                        Content = new ChannelContent()
//                        {
//                            Name = authUser.Email
//                        }
//                    };

//                    return JsonResponse.CreateGood(new ChannelsDTO()
//                    {
//                        Channels = new List<ChannelDTO>()
//                    });
//                }
//                else
//                {
//                    IEnumerable<ChannelDTO> resultRooms = associatedUser.Rooms.Select(cur => new ChannelRoomDTO()
//                    {
//                        Id = cur.Id,
//                        Name = cur.Content.Name,
//                        Description = cur.Content.Description,
//                        Type = ChannelType.Room,
//                        OwnerUser = new ChannelDTO()
//                        {

//                            Id = cur.Owner.Id,
//                            Name = cur.Owner.Content.Name,
//                            Description = cur.Owner.Content.Description,
//                        },
//                        Members = cur.RoomUsers
//                            .Where(roomUser => roomUser.Room == cur)
//                            .Select(roomUser => new ChannelDTO()
//                            {
//                                Id = roomUser.User.Id,
//                                Name = roomUser.User.Content.Name,
//                                Description = roomUser.User.Content.Description,
//                            })
//                    });

//                    IEnumerable<ChannelDTO> resultConnectedUsers = context.UserUsers
//                        .Where(cur => cur.ForUser == associatedUser)
//                        .Select(cur => new ChannelDTO()
//                        {
//                            Id = cur.ConnectedUser.Id,
//                            Name = cur.ConnectedUser.Content.Name,
//                            Description = cur.ConnectedUser.Content.Description
//                        });

//                    return JsonResponse.CreateGood(resultConnectedUsers.Concat(resultConnectedUsers));
//                }
//            }
//            catch (Exception e)
//            {
//                return JsonResponse.CreateBad(HttpStatusCode.InternalServerError, "Server fucked up");
//            }
//        }


//        [HttpGet]
//        [Route("test")]

//        public async Task<IActionResult> Test()
//        {
//            //User user1 = new User()
//            //{
//            //    Content = new ChannelContent()
//            //    {
//            //        Name = "some user"
//            //    }
//            //};

//            //User user2 = new User()
//            //{
//            //    Content = new ChannelContent()
//            //    {
//            //        Name = "another user"
//            //    }
//            //};

//            //Room roomChannel = new Room()
//            //{
//            //    Owner = user1,
//            //    RoomUsers = new List<User>()
//            //    {
//            //        user2
//            //    },
//            //    Content = new ChannelContent()
//            //    {
//            //        Name = "some room"
//            //    }
//            //};

//            //Message fromUser1ToUser2 = new Message()
//            //{
//            //    SenderChannel = user1Channel,
//            //    ReceiverChannel = user2Channel,
//            //    Data = "hello"
//            //};

//            //await context.Rooms.AddRangeAsync(user1Channel, user2Channel, roomChannel);
//            //await context.Messages.AddAsync(fromUser1ToUser2);
//            //await context.SaveChangesAsync();

//            return Ok();
//        }
//    }
//}
