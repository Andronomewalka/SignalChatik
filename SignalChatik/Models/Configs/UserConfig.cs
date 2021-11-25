using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SignalChatik.Models.Configs
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> modelBuilder)
        {
            modelBuilder.HasMany(cur => cur.OwnedRooms)
                .WithOne(cur => cur.Owner)
                .HasForeignKey(cur => cur.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
