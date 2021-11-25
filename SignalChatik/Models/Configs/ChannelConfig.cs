using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SignalChatik.Models.Configs
{
    public class ChannelConfig : IEntityTypeConfiguration<Channel>
    {
        public void Configure(EntityTypeBuilder<Channel> modelBuilder)
        {
            modelBuilder.HasMany(cur => cur.SendedMessages)
                .WithOne(cur => cur.Sender)
                .HasForeignKey(cur => cur.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasMany(cur => cur.ReceivedMessages)
                .WithOne(cur => cur.Receiver)
                .HasForeignKey(cur => cur.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasMany(cur => cur.ForChannels)
                .WithOne(cur => cur.For)
                .HasForeignKey(cur => cur.ForChannelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasMany(cur => cur.ConnectedChannels)
                .WithOne(cur => cur.Connected)
                .HasForeignKey(cur => cur.ConnectedChannelId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
