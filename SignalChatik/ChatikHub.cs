using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalChatik.DTO;
using SignalChatik.DTO.Channel;
using SignalChatik.DTO.Message;
using SignalChatik.Helpers;
using SignalChatik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SignalChatik
{

    [Authorize(Roles = "User")]
    public class ChatikHub : Hub
    {
        private static List<string> connectedUsers = new List<string>();

        protected readonly ChatikContext context;
        public ChatikHub(ChatikContext context)
        {
            this.context = context;
        }

        public async Task SendMessage(MessageRequestDTO request)
        {
            try
            {
                if (request.ChannelId < 0 || string.IsNullOrWhiteSpace(request.Message))
                    await Clients.Caller.SendAsync(ChatikHubMethods.SendMessageFailed.ToString(),
                        JsonResponse.CreateBad(400, $"Channel or Message is invalid"));

                User associatedUser = context.Users
                    .Include(cur => cur.Channel)
                    .FirstOrDefault(cur => cur.Auth.Guid.ToString() == Context.UserIdentifier);

                if (associatedUser == null)
                    await Clients.Caller.SendAsync(ChatikHubMethods.SendMessageFailed.ToString(),
                        JsonResponse.CreateBad(401, $"No auth for user guid found"));

                Channel requestedChannel = context.Channels
                    .Include(cur => cur.Room)
                    .Include(cur => cur.User)
                        .ThenInclude(cur => cur.Auth)
                    .FirstOrDefault(cur => cur.Id == request.ChannelId);

                if (requestedChannel == null)
                    await Clients.Caller.SendAsync(ChatikHubMethods.SendMessageFailed.ToString(),
                        JsonResponse.CreateBad(404, $"Channel doesn't exist"));

                Message message = new Message()
                {
                    Sender = associatedUser.Channel,
                    Receiver = requestedChannel,
                    Data = request.Message,
                    DateTimeUtc = DateTime.UtcNow
                };

                await context.Messages.AddAsync(message);

                IClientProxy receiverUserId = null;

                if (requestedChannel.ChannelTypeId == ChannelType.User)
                {
                    bool isJustConnected = await UpdateConnectionWithUser(associatedUser, requestedChannel);

                    receiverUserId =
                        this.Clients.User(connectedUsers.FirstOrDefault(cur => cur == requestedChannel.User.Auth.Guid.ToString()));

                    if (isJustConnected)
                    {
                        await receiverUserId.SendAsync(ChatikHubMethods.ChannelConnected.ToString(),
                            JsonResponse.CreateGood(new ChannelDTO()
                            {
                                Id = associatedUser.Channel.Id,
                                Name = associatedUser.Channel.Name,
                                Description = associatedUser.Channel.Description,
                                Type = ChannelType.User
                            }));
                    }

                }
                else if (requestedChannel.ChannelTypeId == ChannelType.Room)
                {
                    await UpdateConnectionWithRoom(associatedUser, requestedChannel);

                    List<string> allRoomUsersGuids = await context.ConnectedChannels
                        .Include(cur => cur.Connected)
                            .ThenInclude(cur => cur.User)
                                .ThenInclude(cur => cur.Auth)
                        .AsNoTrackingWithIdentityResolution()
                        .Where(cur => cur.For != associatedUser.Channel && cur.Connected == requestedChannel)
                        .Select(cur => cur.For.User.Auth.Guid.ToString().ToLower())
                        .ToListAsync();

                    receiverUserId = this.Clients.Users(allRoomUsersGuids.Intersect(connectedUsers));
                }

                if (receiverUserId != null)
                {
                    await receiverUserId.SendAsync(ChatikHubMethods.ReceiveMessage.ToString(),
                        JsonResponse.CreateGood(new MessageDTO()
                        {
                            Id = message.Id,
                            User = message.Sender == associatedUser.Channel ? associatedUser.Channel.Name : message.Sender.Name,
                            ReceiverId = requestedChannel.Id,
                            DateUtc = message.DateTimeUtc,
                            Type = MessageType.Receiver,
                            Text = request.Message
                        }));

                    await Clients.Caller.SendAsync(ChatikHubMethods.SendMessageSuccess.ToString(),
                        JsonResponse.CreateGood(new MessageDTO()
                        {
                            Id = message.Id,
                            User = associatedUser.Channel.Name,
                            ReceiverId = requestedChannel.Id,
                            DateUtc = message.DateTimeUtc,
                            Type = MessageType.Sender,
                            Text = request.Message
                        }));
                }
            }
            catch (Exception e)
            {
                await Clients.Caller.SendAsync(ChatikHubMethods.SendMessageSuccess.ToString(),
                    JsonResponse.CreateBad(HttpStatusCode.InternalServerError, "Server fucked up"));
            }
        }

        private async Task<bool> UpdateConnectionWithUser(User fromUser, Channel toChannel)
        {
            // connect receiver channel, if it's not connected yet
            bool isChannelJustConnected = false;

            var connectedBackChannel = await context.ConnectedChannels
                        .FirstOrDefaultAsync(cur => cur.For == fromUser.Channel &&
                            cur.Connected == toChannel &&
                            !cur.IsConnectedBack);

            ConnectedChannel connectedChannel = await context.ConnectedChannels
                        .FirstOrDefaultAsync(cur => (cur.For == fromUser.Channel && cur.Connected == toChannel) ||
                                                    (cur.For == toChannel && cur.Connected == fromUser.Channel));

            if (connectedChannel == null)
            {
                connectedChannel = new ConnectedChannel()
                {
                    For = toChannel,
                    Connected = fromUser.Channel
                };
                await context.ConnectedChannels.AddAsync(connectedChannel);
                isChannelJustConnected = true;
            }
            else if (!connectedChannel.IsConnectedBack)
            {
                connectedChannel.IsConnectedBack = true;
                isChannelJustConnected = true;
            }
            connectedChannel.LastInteractionTime = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return isChannelJustConnected;
        }

        private async Task UpdateConnectionWithRoom(User fromUser, Channel toChannel)
        {
            var connectedChannels = context.ConnectedChannels
                .Include(cur => cur.Connected)
                .Where(cur => cur.Connected == toChannel && cur.Connected != fromUser.Channel);

            await connectedChannels.ForEachAsync(cur =>
            {
                cur.IsConnectedBack = true;
                cur.LastInteractionTime = DateTime.UtcNow;
            });
            await context.SaveChangesAsync();
        }

        public override Task OnConnectedAsync()
        {
            connectedUsers.Add(Context.UserIdentifier);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            connectedUsers.Remove(Context.UserIdentifier);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
