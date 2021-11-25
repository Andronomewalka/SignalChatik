using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalChatik.DTO;
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
                    Data = request.Message
                };

                await context.Messages.AddAsync(message);

                bool isChannelConnected = false;
                // connect receiver channel, if it's not connected yet
                if (!await context.ConnectedChannels.AnyAsync(cur =>
                    cur.For == requestedChannel &&
                    cur.Connected == associatedUser.Channel))
                {
                    await context.ConnectedChannels.AddAsync(new ConnectedChannel()
                    {
                        For = requestedChannel,
                        Connected = associatedUser.Channel
                    });
                    isChannelConnected = true;
                }
                await context.SaveChangesAsync();

                await Clients.Caller.SendAsync(ChatikHubMethods.SendMessageSuccess.ToString(),
                    JsonResponse.CreateGood(new MessageDTO()
                    {
                        Id = message.Id,
                        User = associatedUser.Channel.Name,
                        ReceiverId = requestedChannel.Id,
                        DateUtc = DateTime.UtcNow,
                        Type = MessageType.Sender,
                        Text = request.Message
                    }));

                //if send to user
                if (requestedChannel.ChannelTypeId == ChannelType.User)
                {
                    IClientProxy receiverUserId =
                        this.Clients.User(connectedUsers.FirstOrDefault(cur => cur == requestedChannel.User.Auth.Guid.ToString()));

                    if (isChannelConnected)
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
                    await receiverUserId.SendAsync(ChatikHubMethods.ReceiveMessage.ToString(),
                        JsonResponse.CreateGood(new MessageDTO()
                        {
                            Id = message.Id,
                            User = requestedChannel.Name,
                            ReceiverId = requestedChannel.Id,
                            DateUtc = DateTime.UtcNow,
                            Type = MessageType.Receiver,
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
